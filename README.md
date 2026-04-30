# Evalify System
# Evalify — Frontend Developer Guide

## What is Evalify?

Evalify is an AI-powered platform that allows teachers to automatically evaluate handwritten student exam answers. The teacher uploads scanned answer sheets, defines where each question's answer is on the sheet, provides a model answer, and the system grades everything automatically.

---

## Base URL

```
https://localhost:7065/api/v1
```
```
http://localhost:5140/api/v1
```
> Change this to the actual server URL when deployed.

## API Explorer

- **Scalar UI:** `https://localhost:7065/scalar/v1`
- **Swagger:** `https://localhost:7065/swagger`

---

## Authentication

All endpoints except `register` and `login` require a JWT token:

```
Authorization: Bearer {token}
```

Store the token after login:

```javascript
localStorage.setItem("token", response.token);
```

Attach to every request:

```javascript
const headers = {
  "Authorization": `Bearer ${localStorage.getItem("token")}`,
  "Content-Type": "application/json"
};
```

---

## Error Handling

All errors return `ProblemDetails` format:

```json
{
  "title": "Template does not exist.",
  "status": 404
}
```

| Status Code | Meaning | Action |
|---|---|---|
| 400 | Validation error | Show `title` to user |
| 401 | Not authenticated | Redirect to login |
| 403 | Forbidden | Show access denied |
| 404 | Not found | Show not found message |
| 409 | Conflict | e.g. email already exists |
| 500 | Server error | Show generic error |

---

## Pages & Flow

```
Login / Register
      ↓
Dashboard — list of templates
      ↓
Template Creator — upload image + draw bounding boxes
      ↓
Upload Papers — upload student answer sheets
      ↓
Results — view grades + manual adjustment + export CSV
```

---

## API Endpoints

---

### 1. Auth

#### Register
```
POST /auth/register
```

```json
// Request
{
  "fullName": "Ahmed Mohamed",
  "email":    "ahmed@evalify.com",
  "password": "Pass@1234"
}

// Response 201
{
  "userId":   "guid-string",
  "fullName": "Ahmed Mohamed",
  "email":    "ahmed@evalify.com"
}
```

#### Login
```
POST /auth/login
```

```json
// Request
{
  "email":    "ahmed@evalify.com",
  "password": "Pass@1234"
}

// Response 200
{
  "token":    "eyJhbGci...",
  "userId":   "guid-string",
  "fullName": "Ahmed Mohamed",
  "email":    "ahmed@evalify.com"
}
```

---

### 2. Templates

#### Get All Templates
```
GET /templates
Authorization: Bearer {token}
```

```json
// Response 200
[
  {
    "templateId": 1,
    "name":       "Final Exam - CS101",
    "imageUrl":   "https://localhost:7065/uploads/templates/xxx.jpg",
    "width":      2480,
    "height":     3508,
    "createdAt":  "2025-12-01T10:00:00Z"
  }
]
```

#### Create Template
```
POST /templates
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

```
name:  "Final Exam - CS101"
image: [FILE: jpg/png]
```

```json
// Response 201
{
  "templateId": 1,
  "name":       "Final Exam - CS101",
  "imageUrl":   "https://localhost:7065/uploads/templates/xxx.jpg",
  "width":      2480,
  "height":     3508,
  "createdAt":  "2025-12-01T10:00:00Z"
}
```

> **Important:** Save `width` and `height` from the response — you need them for bounding box scale calculation.

---

### 3. Template Questions (Bounding Boxes)

#### Get Questions
```
GET /templates/{templateId}/questions
Authorization: Bearer {token}
```

```json
// Response 200
[
  {
    "questionId":    1,
    "questionIndex": 1,
    "x":             120,
    "y":             200,
    "width":         800,
    "height":        150,
    "modelAnswer":   "The water cycle consists of...",
    "mark":          10.0
  }
]
```

#### Save Questions (Full Replace)
```
POST /templates/{templateId}/questions
Authorization: Bearer {token}
Content-Type: application/json
```

```json
// Request
{
  "questions": [
    {
      "questionIndex": 1,
      "x":             120,
      "y":             200,
      "width":         800,
      "height":        150,
      "modelAnswer":   "The water cycle consists of...",
      "mark":          10.0
    },
    {
      "questionIndex": 2,
      "x":             120,
      "y":             380,
      "width":         800,
      "height":        150,
      "modelAnswer":   "Photosynthesis is the process...",
      "mark":          15.0
    }
  ]
}

// Response 200
// Empty body — check status code
```

> **Important:** This is a **full replace** — every save deletes old questions and inserts new ones. Always send the complete list of boxes on every save.

---

### 4. Bounding Box Scale Calculation

The image is displayed smaller on screen than its real size. Before sending coordinates to the backend, scale them to real pixel values:

```javascript
// Before sending to backend
const scaleX = originalWidth  / displayedWidth;   // e.g. 2480 / 620 = 4
const scaleY = originalHeight / displayedHeight;  // e.g. 3508 / 877 = 4

const realBox = {
  x:      Math.round(drawnBox.x      * scaleX),
  y:      Math.round(drawnBox.y      * scaleY),
  width:  Math.round(drawnBox.width  * scaleX),
  height: Math.round(drawnBox.height * scaleY),
};

// When loading saved boxes to display on screen, reverse:
const displayBox = {
  x:      Math.round(savedBox.x      / scaleX),
  y:      Math.round(savedBox.y      / scaleY),
  width:  Math.round(savedBox.width  / scaleX),
  height: Math.round(savedBox.height / scaleY),
};
```

Where:
- `originalWidth` / `originalHeight` → from the `POST /templates` response
- `displayedWidth` / `displayedHeight` → the actual rendered size of the `<img>` element on screen

---

### 5. Student Papers

#### Upload Papers (Bulk)
```
POST /templates/{templateId}/papers
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

```javascript
// React example
const formData = new FormData();
selectedFiles.forEach(file => formData.append("images", file));

await fetch(`/api/v1/templates/${templateId}/papers`, {
  method: "POST",
  headers: { "Authorization": `Bearer ${token}` },
  body: formData
  // Do NOT set Content-Type manually — browser sets it with boundary automatically
});
```

> **Important:** The filename **is** the student code. Name files like `CS2101001.jpg` — the backend extracts the student code from the filename automatically.

```json
// Response 202
{
  "uploaded": [
    { "studentPaperId": 1, "studentCode": "CS2101001", "status": "Pending" },
    { "studentPaperId": 2, "studentCode": "CS2101002", "status": "Pending" }
  ],
  "failed": []
}
```

#### Get All Papers
```
GET /templates/{templateId}/papers
Authorization: Bearer {token}
```

```json
// Response 200
[
  {
    "studentPaperId": 1,
    "studentCode":    "CS2101001",
    "status":         "Done",
    "totalGrade":     22.5,
    "createdAt":      "2025-12-10T09:00:00Z"
  },
  {
    "studentPaperId": 2,
    "studentCode":    "CS2101002",
    "status":         "Processing",
    "totalGrade":     null,
    "createdAt":      "2025-12-10T09:05:00Z"
  }
]
```

**Paper Status Values:**

| Status | Meaning |
|---|---|
| `Pending` | Uploaded, waiting to be processed |
| `Processing` | AI is currently grading |
| `Done` | Grading complete — results available |
| `Failed` | Something went wrong |

> **Polling:** Poll `GET /templates/{id}/papers` every **5 seconds** for papers with `status = Processing` or `Pending`.

```javascript
// Polling example
const pollPapers = (templateId) => {
  const interval = setInterval(async () => {
    const papers = await fetchPapers(templateId);
    const stillProcessing = papers.some(
      p => p.status === "Processing" || p.status === "Pending"
    );
    if (!stillProcessing) clearInterval(interval);
    setPapers(papers);
  }, 5000);
};
```

---

### 6. Results

#### Get Results for One Student
```
GET /papers/{studentPaperId}/results
Authorization: Bearer {token}
```

```json
// Response 200
{
  "studentPaperId": 1,
  "studentCode":    "CS2101001",
  "totalGrade":     22.5,
  "imageUrl":       "https://localhost:7065/uploads/papers/xxx.jpg",
  "status":         "Done",
  "answers": [
    {
      "answerId":      10,
      "questionIndex": 1,
      "modelAnswer":   "The water cycle consists of...",
      "extractedText": "Text read by AI from the handwriting",
      "grade":         8.7,
      "maxMark":       10.0
    },
    {
      "answerId":      11,
      "questionIndex": 2,
      "modelAnswer":   "Photosynthesis is...",
      "extractedText": null,
      "grade":         13.8,
      "maxMark":       15.0
    }
  ]
}
```

> `extractedText` can be `null` — show it if present, hide the field if null.

#### Adjust Grade Manually
```
PUT /answers/{answerId}/grade
Authorization: Bearer {token}
Content-Type: application/json
```

```json
// Request
{ "grade": 9.0 }

// Response 200
{ "newTotalGrade": 23.8 }
```

> After adjusting, update the displayed `totalGrade` with `newTotalGrade` from the response.

---

### 7. Export CSV
```
GET /templates/{templateId}/export
Authorization: Bearer {token}
```

```javascript
// Download file in React
const exportCSV = async (templateId) => {
  const response = await fetch(`/api/v1/templates/${templateId}/export`, {
    headers: { "Authorization": `Bearer ${token}` }
  });
  const blob = await response.blob();
  const url  = URL.createObjectURL(blob);
  const a    = document.createElement("a");
  a.href     = url;
  a.download = "results.csv";
  a.click();
};
```

CSV format:
```
StudentCode, Q1 Grade, Q1 MaxMark, Q2 Grade, Q2 MaxMark, TotalGrade
CS2101001,   7.5,      10,         12.3,      15,         19.8
CS2101002,   8.0,      10,         13.5,      15,         21.5
```

> Only exports papers with `status = Done`. Pending and Failed papers are excluded.

---

## Static Files (Images)

Template and paper images are served directly from the backend. Use the `imageUrl` field returned from the API:

```jsx
// Template image
<img src={template.imageUrl} />

// Student paper image (for teacher review)
<img src={paper.imageUrl} />
```

URLs format:
```
https://localhost:7065/uploads/templates/{filename}
https://localhost:7065/uploads/papers/{filename}
```

---

## CORS

The backend allows requests from any origin during development (`AllowAll` policy). No additional CORS setup needed on the frontend.

---

## Backend Setup (Run Locally)

1. Install **.NET 10 SDK** from [dot.net](https://dot.net)
2. Install **SQL Server Express** from Microsoft
3. Clone the repo and open terminal in the solution root
4. Edit `Evalify.API/appsettings.json` — change the server name:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_PC_NAME\\SQLEXPRESS;Database=EvalifyDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

5. Run migration:

```bash
dotnet tool install --global dotnet-ef


dotnet ef database update --project Evalify.Infrastructure --startup-project Evalify.API
```

6. Run the API:

```bash
dotnet run --project Evalify.API
```

7. API runs on: `https://localhost:7065` & `http://localhost:5140`
8. Scalar UI: `https://localhost:7065/scalar/v1`
9. Swagger: `https://localhost:7065/swagger`

---
Testing the Upload Endpoint
Swagger and Scalar have issues with file uploads. Use Postman or a .http file instead.
Postman:
```
Method: POST
URL: http://localhost:5140/api/v1/templates/{templateId}/papers
Headers: Authorization: Bearer {token}
Body: form-data

Key: images → Type: File → Value: select your jpg files
```

```
VS Code .http file:
httpPOST http://localhost:5140/api/v1/templates/{{templateId}}/papers
Authorization: Bearer {{token}}
Content-Type: multipart/form-data; boundary=----Boundary

------Boundary
Content-Disposition: form-data; name="images"; filename="CS2101001.jpg"
Content-Type: image/jpeg

< C:\path\to\CS2101001.jpg
------Boundary--
```

## Quick Reference

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/auth/register` | No | Register teacher |
| POST | `/auth/login` | No | Login + get token |
| GET | `/templates` | Yes | Get all templates |
| POST | `/templates` | Yes | Create template — returns TemplateDto |
| GET | `/templates/{id}/questions` | Yes | Get bounding boxes |
| POST | `/templates/{id}/questions` | Yes | Save bounding boxes (full replace) |
| GET | `/templates/{id}/papers` | Yes | Get all papers + status |
| POST | `/templates/{id}/papers` | Yes | Upload answer sheets (bulk) |
| GET | `/papers/{id}/results` | Yes | Get student results |
| PUT | `/answers/{id}/grade` | Yes | Adjust grade manually |
| GET | `/templates/{id}/export` | Yes | Export all results as CSV |