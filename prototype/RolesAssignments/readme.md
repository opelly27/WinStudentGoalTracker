# User → Student Assignments

| User (X-User-Id)     | Student 101 (A)    | Student 102 (B)    | Student 103 (C)  | Student 104 (D)  |
|----------------------|--------------------|--------------------|------------------|------------------|
| **1 — Ms. Rivera**   | ✅ PrimaryTeacher  | ✅ PrimaryTeacher  | ❌ No access     | ❌ No access     |
| **2 — Mr. Daniels**  | ✅ Paraeducator    | ❌ No access       | ❌ No access     | ❌ No access     |
| **3 — Dr. Patel**    | ✅ Supervisor      | ✅ Supervisor      | ✅ Supervisor    | ✅ Supervisor    |

# What Each Assignment Type Can Do

| Action                 | PrimaryTeacher   | Paraeducator       | Supervisor  |
|------------------------|------------------|--------------------|-------------|
| View student           | ✅               | ✅                 | ✅          |
| Create goal            | ✅               | ❌                 | ❌          |
| Edit goal              | ✅               | ❌                 | ❌          |
| Add progress entry     | ✅               | ✅                 | ❌          |
| Edit progress entry    | ✅ any entry     | ✅ own entries only | ❌          |
| Delete progress entry  | ✅ any entry     | ✅ own entries only | ❌          |

# Quick Test Scenarios

- **User 1, GET /api/students**             → sees A & B (not C or D)
- **User 2, GET /api/students**             → sees A only
- **User 3, GET /api/students**             → sees all four (A, B, C, D)
- **User 2, POST goal for student 101**     → 403 Forbidden (paras can't create goals)
- **User 2, PUT entry 2 for student 101**   → ✅ (it's his own entry)
- **User 2, PUT entry 1 for student 101**   → 403 (entry 1 was created by user 1)
