# 人脸识别系统

## 项目结构

```
.
├── face_service/          # Python人脸识别后端服务
│   ├── app.py            # Flask API 主程序
│   ├── requirements.txt  # Python依赖
│   └── init_db.sql       # 数据库初始化脚本
│
└── FaceRecognitionWeb/   # ASP.NET前端项目
    ├── Controllers/       # 控制器
    └── Views/            # 视图页面
```

## 运行环境

### 1. MySQL数据库

```sql
-- 执行 init_db.sql 创建数据库和表
mysql -u root -p < face_service/init_db.sql
```

### 2. Python后端服务

```bash
cd face_service
pip install -r requirements.txt
python app.py
```

服务将在 http://localhost:5000 启动

### 3. ASP.NET前端

```bash
cd FaceRecognitionWeb
dotnet run
```

前端将在 http://localhost:5000 启动（默认）

## API接口

| 接口 | 方法 | 参数 | 说明 |
|------|------|------|------|
| /api/register | POST | username, image | 用户注册 |
| /api/login | POST | image | 人脸登录 |
| /api/users | GET | - | 获取用户列表 |

## 功能说明

1. **用户注册**: 上传用户名和照片，系统自动提取人脸特征并存入数据库
2. **人脸登录**: 上传照片，系统与数据库中的人脸特征进行匹配
3. **用户管理**: 登录后可查看所有已注册用户信息
