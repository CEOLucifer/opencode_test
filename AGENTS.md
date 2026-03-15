# AGENTS.md - Agentic Coding Guidelines

This document provides guidelines for agentic coding agents working in this repository.

## Project Overview

Face recognition system with three components:
- **Python Flask backend**: `face_service/` - REST API for face registration/login
- **Node.js utilities**: Root level - docx processing scripts
- **ASP.NET Core frontend**: `FaceRecognitionWeb/` - Web UI

---

## Build, Test, and Development Commands

### Python Backend (face_service/)

```bash
# Install dependencies
pip install -r face_service/requirements.txt

# Run the Flask application
python face_service/app.py

# Run with debug mode (auto-reload)
FLASK_DEBUG=1 python face_service/app.py

# Single test (pytest)
pytest face_service/test_file.py::test_function_name -v

# Run all tests
pytest face_service/ -v

# Linting (flake8)
flake8 face_service/ --max-line-length=120

# Type checking (mypy)
mypy face_service/
```

### Node.js Utilities

```bash
# Install dependencies
npm install

# Run the docx reader script
node read_docx.js

# Linting (ESLint)
npx eslint .
```

### ASP.NET Core Frontend (FaceRecognitionWeb/)

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run single test
dotnet test --filter "FullyQualifiedName~TestNamespace.TestClass.TestMethod"

# Run all tests
dotnet test

# Linting (formatting check)
dotnet format --verify-no-changes

# Build release
dotnet publish -c Release
```

---

## Code Style Guidelines

### General Principles
- Keep code modular and focused on single responsibility
- Write self-documenting code with clear variable/function names
- Handle errors gracefully with appropriate logging
- Never commit secrets, API keys, or credentials
- Use configuration files for environment-specific settings

---

### Python (Flask Backend)

**Imports**: Standard library → third-party → local (with blank lines between)
```python
import json
from datetime import datetime

import numpy as np
import cv2
from flask import Flask, request, jsonify

from .models import User
```

**Formatting**: 4 spaces, max 120 chars, use f-strings, avoid trailing whitespace

**Types**: Use type hints, prefer `Optional[X]` for Python 3.9 compatibility, use `Union[X, Y]` instead of `X | Y`

**Naming**: `snake_case` (functions/variables), `PascalCase` (classes), `UPPER_SNAKE_CASE` (constants)

**Error Handling**: Return proper HTTP status codes, log errors with appropriate levels, never expose stack traces to clients

**Database**: Use SQLAlchemy ORM, always use transactions, prefer parameterized queries

---

### JavaScript/Node.js

**Imports**: Use CommonJS (`require`), group: built-in → external → local

**Formatting**: 2 spaces, semicolons, single quotes, max 100 chars

**Naming**: `camelCase` (vars/functions), `PascalCase` (classes), SCREAMING_SNAKE_CASE (constants)

**Error Handling**: Handle promise rejections, use try/catch for async operations, always have catch blocks

**File Organization**: One export per file preferred, use barrel files (index.js) for re-exports

---

### C# (.NET)

**Imports**: Use implicit usings (enabled in project), group by namespace

**Formatting**: 4 spaces, `_camelCase` (private fields), `PascalCase` (public members)

**Types**: Enable nullable reference types, use `var`, prefer LINQ over loops, use records for DTOs

**Naming**: `PascalCase` (methods/properties/classes), `camelCase` (parameters/local vars), `_camelCase` (private fields)

**Error Handling**: Use try/catch, return appropriate IActionResult, use logging injection, prefer custom exception types

**Patterns**: Use dependency injection, follow DRY, prefer composition over inheritance

---

## Testing Guidelines

- Write unit tests for business logic, integration tests for API endpoints
- Use descriptive test names: `test_function_name_scenario_expected_result`
- Follow Arrange-Act-Assert pattern
- Mock external dependencies (database, file system, APIs)
- Keep tests focused and fast (< 100ms per test)
- Aim for >70% code coverage on critical paths

---

## Git Conventions

- Use meaningful commit messages: "Add user authentication" not "Fixed stuff"
- Keep commits atomic (one logical change per commit)
- Create feature branches: `feature/add-login`, `fix/face-detection-error`
- Run linting before committing
- Never commit: secrets, node_modules, bin/, obj/, .env files
- Use conventional commits: `feat:`, `fix:`, `refactor:`, `test:`, `docs:`

---

## API Design

### REST Endpoints
- Use nouns for resources: `/users`, `/faces`, not `/getUsers`
- Use HTTP methods correctly: GET (read), POST (create), PUT (update), DELETE (remove)
- Return proper status codes: 200 (OK), 201 (Created), 400 (Bad Request), 401 (Unauthorized), 404 (Not Found), 500 (Server Error)
- Always return JSON with consistent structure

---

## Security Guidelines

- Never log sensitive data (passwords, tokens, PII)
- Validate all input data, never trust user input
- Use parameterized queries to prevent SQL injection
- Store passwords with hashing (bcrypt/argon2), never plaintext
- Use HTTPS in production, secure cookies
- Implement rate limiting on public endpoints

---

## Additional Notes

- Python backend uses `face_recognition` library which requires dlib
- Ensure MySQL is running before starting the backend
- .NET project targets .NET 9.0
- Frontend connects to Python backend on port 5000
- Use environment variables for configuration, not hardcoded values
