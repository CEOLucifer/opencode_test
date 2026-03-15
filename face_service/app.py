import base64
import io
import numpy as np
import cv2
import face_recognition
from flask import Flask, request, jsonify
import pymysql
from datetime import datetime

app = Flask(__name__)

DB_CONFIG = {
    'host': 'localhost',
    'user': 'root',
    'password': '1234',
    'database': 'face_recognition',
    'charset': 'utf8mb4'
}

def get_db_connection():
    return pymysql.connect(**DB_CONFIG)

def base64_to_image(base64_str):
    if base64_str.startswith('data:image'):
        base64_str = base64_str.split(',')[1]
    img_data = base64.b64decode(base64_str)
    nparr = np.frombuffer(img_data, np.uint8)
    img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    return img

def image_to_base64(img):
    _, buffer = cv2.imencode('.jpg', img)
    return base64.b64encode(buffer).decode('utf-8')

def get_face_encoding(img):
    rgb_img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    encodings = face_recognition.face_encodings(rgb_img)
    if encodings:
        return encodings[0].tolist()
    return None

def compare_faces(known_encoding, face_encoding, tolerance=0.4):
    if known_encoding is None or face_encoding is None:
        return False
    results = face_recognition.compare_faces([np.array(known_encoding)], np.array(face_encoding), tolerance=tolerance)
    return results[0]

@app.route('/api/register', methods=['POST'])
def register():
    try:
        data = request.get_json()
        username = data.get('username')
        image_base64 = data.get('image')

        if not username or not image_base64:
            return jsonify({'status': 'error', 'message': 'Missing username or image'}), 400

        img = base64_to_image(image_base64)
        if img is None:
            return jsonify({'status': 'error', 'message': 'Invalid image'}), 400

        face_encoding = get_face_encoding(img)
        if face_encoding is None:
            return jsonify({'status': 'error', 'message': 'No face detected'}), 400

        conn = get_db_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT id FROM user WHERE username = %s", (username,))
        if cursor.fetchone():
            cursor.close()
            conn.close()
            return jsonify({'status': 'error', 'message': 'Username already exists'}), 400

        import json
        feature_str = json.dumps(face_encoding)
        create_time = datetime.now().strftime('%Y-%m-%d %H:%M:%S')

        cursor.execute(
            "INSERT INTO user (username, face_feature, create_time) VALUES (%s, %s, %s)",
            (username, feature_str, create_time)
        )
        conn.commit()
        cursor.close()
        conn.close()

        return jsonify({'status': 'success', 'message': 'Registration successful'})
    except Exception as e:
        return jsonify({'status': 'error', 'message': str(e)}), 500

@app.route('/api/login', methods=['POST'])
def login():
    try:
        data = request.get_json()
        image_base64 = data.get('image')

        if not image_base64:
            return jsonify({'status': 'error', 'message': 'Missing image'}), 400

        img = base64_to_image(image_base64)
        if img is None:
            return jsonify({'status': 'error', 'message': 'Invalid image'}), 400

        face_encoding = get_face_encoding(img)
        if face_encoding is None:
            return jsonify({'status': 'error', 'message': 'No face detected'}), 400

        conn = get_db_connection()
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        cursor.execute("SELECT id, username, face_feature FROM user")
        users = cursor.fetchall()
        cursor.close()
        conn.close()

        import json
        for user in users:
            known_encoding = json.loads(user['face_feature'])
            if compare_faces(known_encoding, face_encoding):
                return jsonify({
                    'status': 'success',
                    'user_id': user['id'],
                    'username': user['username']
                })

        return jsonify({'status': 'error', 'message': 'Face not recognized'}), 401
    except Exception as e:
        return jsonify({'status': 'error', 'message': str(e)}), 500

@app.route('/api/users', methods=['GET'])
def get_users():
    try:
        conn = get_db_connection()
        cursor = conn.cursor(pymysql.cursors.DictCursor)
        cursor.execute("SELECT id, username, create_time FROM user ORDER BY create_time DESC")
        users = cursor.fetchall()
        cursor.close()
        conn.close()

        for user in users:
            if user['create_time']:
                user['create_time'] = user['create_time'].strftime('%Y-%m-%d %H:%M:%S')

        return jsonify(users)
    except Exception as e:
        return jsonify({'status': 'error', 'message': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
