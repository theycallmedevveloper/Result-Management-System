let selectedStudentId = null;

// Add marks for a single subject
async function addMarks() {
    if (!selectedStudentId) {
        alert("Please select a student from suggestions");
        return;
    }

    const subjectId = document.getElementById("subjectSelect").value;
    const marks = document.getElementById("marks").value;

    if (!subjectId || !marks) {
        alert("Please fill in all fields");
        return;
    }

    try {
        const res = await fetch("https://localhost:7240/api/marks/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({
                studentId: selectedStudentId,
                subjectId,
                marksObtained: marks
            })
        });

        if (!res.ok) {
            alert("Failed to add marks");
            return;
        }

        alert("Marks added successfully");

        // Clear form after successful submission
        document.getElementById("studentSearch").value = "";
        document.getElementById("subjectSelect").value = "";
        document.getElementById("marks").value = "";
        document.getElementById("selectedStudent").style.display = "none";
        selectedStudentId = null;

    } catch (error) {
        alert(" Error: " + error.message);
    }
}

// Fetch student suggestions based on search query
async function suggestStudents() {
    const q = document.getElementById("studentSearch").value;

    if (q.length < 1) {
        document.getElementById("suggestions").innerHTML = "";
        return;
    }

    try {
        const res = await fetch(
            `https://localhost:7240/api/students/suggest?q=${encodeURIComponent(q)}`,
            { credentials: "include" }
        );

        if (!res.ok) {
            return;
        }

        const students = await res.json();
        const list = document.getElementById("suggestions");
        list.innerHTML = "";

        if (students.length === 0) {
            list.innerHTML = `
                <li class="list-group-item text-muted">
                    No students found
                </li>
            `;
            return;
        }

        // Create clickable list items for each student
        students.forEach(s => {
            const li = document.createElement("li");
            li.className = "list-group-item list-group-item-action";
            li.innerText = `${s.firstName} ${s.lastName} (${s.rollNumber})`;
            li.onclick = () => selectStudent(s);
            list.appendChild(li);
        });
    } catch (error) {
        console.error("Error fetching students:", error);
    }
}

// Handle student selection from suggestions
function selectStudent(student) {
    selectedStudentId = student.studentId;
    document.getElementById("studentSearch").value =
        `${student.firstName} ${student.lastName}`;

    const selectedDiv = document.getElementById("selectedStudent");
    selectedDiv.innerText = `Selected: ${student.firstName} ${student.lastName}`;
    selectedDiv.style.display = "block";

    document.getElementById("suggestions").innerHTML = "";
    const firstSelect = document.querySelector(".subjectSelect");
    loadSubjects(firstSelect);
}

// Disable already selected subjects across all rows
function handleSubjectChange() {
    const selects = document.querySelectorAll(".subjectSelect");
    const selectedValues = Array.from(selects)
        .map(s => s.value)
        .filter(v => v !== "");

    selects.forEach(select => {
        Array.from(select.options).forEach(opt => {
            if (
                opt.value !== "" &&
                opt.value !== select.value &&
                selectedValues.includes(opt.value)
            ) {
                opt.disabled = true;
            } else {
                opt.disabled = false;
            }
        });
    });
}

// Load all available subjects into dropdown
async function loadSubjects(selectElement) {
    const res = await fetch("https://localhost:7240/api/subjects", {
        credentials: "include"
    });

    const subjects = await res.json();

    selectElement.innerHTML = `<option value="">-- Select Subject --</option>`;

    subjects.forEach(s => {
        const opt = document.createElement("option");
        opt.value = s.subjectId;
        opt.textContent = s.subjectName;
        selectElement.appendChild(opt);
    });

    handleSubjectChange();
}

// Remove a subject-marks row
function removeSubjectRow(button) {
    const row = button.closest(".mark-row");
    row.remove();

    // Re-enable subjects after removal
    handleSubjectChange();
}

// Load all student results for admin view
async function loadResults() {
    try {
        const res = await fetch("https://localhost:7240/api/marks/all-results", {
            credentials: "include"
        });

        if (!res.ok) {
            alert("Failed to load results");
            return;
        }

        const data = await res.json();
        const tbody = document.getElementById("results");
        tbody.innerHTML = "";

        if (data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="text-center text-muted">
                        No results found
                    </td>
                </tr>
            `;
            return;
        }

        // Group results by student for better display
        const studentMap = {};
        data.forEach(r => {
            if (!studentMap[r.studentName]) {
                studentMap[r.studentName] = [];
            }
            studentMap[r.studentName].push({
                subject: r.subjectName,
                marks: r.marksObtained
            });
        });

        // Render grouped results with rowspan for student names
        Object.keys(studentMap).forEach(studentName => {
            const subjects = studentMap[studentName];

            // First row with student name and rowspan
            tbody.innerHTML += `
                <tr>
                    <td rowspan="${subjects.length}" class="align-middle"><strong>${studentName}</strong></td>
                    <td>${subjects[0].subject}</td>
                    <td>${subjects[0].marks}</td>
                </tr>
            `;

            // Remaining subjects without student name
            for (let i = 1; i < subjects.length; i++) {
                tbody.innerHTML += `
                    <tr>
                        <td>${subjects[i].subject}</td>
                        <td>${subjects[i].marks}</td>
                    </tr>
                `;
            }
        });
    } catch (error) {
        alert("Error loading results: " + error.message);
    }
}

// Logout admin user
async function logout() {
    try {
        await fetch("https://localhost:7240/api/auth/logout", {
            method: "POST",
            credentials: "include"
        });
        window.location.href = "index.html";
    } catch (error) {
        console.error("Logout error:", error);
        window.location.href = "index.html";
    }
}

// Add new subject-marks input row
function addMoreSubjectRow() {
    const container = document.getElementById("marksContainer");

    const row = document.createElement("div");
    row.className = "row g-2 mark-row align-items-center";

    row.innerHTML = `
        <div class="col-md-5">
            <select class="form-control subjectSelect" onchange="handleSubjectChange()">
                <option value="">-- Select Subject --</option>
            </select>
        </div>
        <div class="col-md-5">
            <input type="number"
                   class="form-control marksInput"
                   placeholder="Enter marks">
        </div>
        <div class="col-md-1 d-flex align-items-center justify-content-center">
            <button class="btn btn-danger btn-sm" onclick="removeSubjectRow(this)" style="width: 40px; height: 38px; padding: 2px">
                Remove  
            </button>
        </div>
    `;

    container.appendChild(row);

    // Load subjects ONLY for the new row
    const newSelect = row.querySelector(".subjectSelect");
    loadSubjects(newSelect);
}

// Create a new student account
async function createStudent() {
    const msgEl = document.getElementById("createMsg");

    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const classVal = document.getElementById("class")?.value?.trim() || "";
    const rollNumber = document.getElementById("rollNumber").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!firstName || !lastName || !classVal || !rollNumber || !email || !password) {
        msgEl.className = "alert alert-danger mt-3";
        msgEl.textContent = "Please fill in all fields.";
        msgEl.style.display = "block";
        return;
    }

    // Ensure rollNumber is a string (not a number)
    const payload = {
        firstName: firstName,
        lastName: lastName,
        class: classVal,
        rollNumber: String(rollNumber),  // Convert to string explicitly
        email: email,
        password: password
    };

    try {
        const res = await fetch("https://localhost:7240/api/students/create", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(payload)
        });

        // Check response content type
        const contentType = res.headers.get("content-type");
        let responseData;

        if (contentType && contentType.includes("application/json")) {
            responseData = await res.json();
        } else {
            const text = await res.text();
            console.error("Non-JSON response:", text);
            throw new Error(`Server returned: ${text}`);
        }

        if (!res.ok) {
            // Extract validation errors if available
            if (responseData.errors) {
                const errorMsgs = Object.values(responseData.errors)
                    .flat()
                    .join(", ");
                throw new Error(errorMsgs);
            }
            throw new Error(responseData.message || responseData.title || "Failed to create student");
        }

        // Success - show confirmation
        msgEl.className = "alert alert-success mt-3";
        msgEl.textContent = `Student created successfully! ID: ${responseData.studentId} | Username: ${responseData.username}`;
        msgEl.style.display = "block";

        // Clear form
        document.getElementById("firstName").value = "";
        document.getElementById("lastName").value = "";
        document.getElementById("class").value = "";
        document.getElementById("rollNumber").value = "";
        document.getElementById("email").value = "";
        document.getElementById("password").value = "";

        // Auto-close modal after 3 seconds
        setTimeout(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById("createStudentModal"));
            if (modal) {
                modal.hide();
            }
            msgEl.style.display = "none";
        }, 3000);

    } catch (err) {
        console.error("Create student error:", err);
        msgEl.className = "alert alert-danger mt-3";
        msgEl.textContent = "Error: " + err.message;
        msgEl.style.display = "block";
    }
}

// Add marks for multiple subjects at once
async function addMultipleMarks() {
    if (!selectedStudentId) {
        alert("Please select a student");
        return;
    }

    const rows = document.querySelectorAll(".mark-row");
    const payloads = [];

    rows.forEach(row => {
        const subjectId = row.querySelector(".subjectSelect").value;
        const marks = row.querySelector(".marksInput").value;

        if (subjectId && marks) {
            payloads.push({ subjectId, marksObtained: marks });
        }
    });

    if (payloads.length === 0) {
        alert("Please enter at least one subject & marks");
        return;
    }

    try {
        // Add marks for each subject sequentially
        for (const p of payloads) {
            const res = await fetch("https://localhost:7240/api/marks/add", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({
                    studentId: selectedStudentId,
                    subjectId: p.subjectId,
                    marksObtained: p.marksObtained
                })
            });

            if (!res.ok) {
                throw new Error("Failed to add marks");
            }
        }

        alert("All marks added successfully");

        // Reset UI
        document.getElementById("marksContainer").innerHTML = "";
        addMoreSubjectRow();
        document.getElementById("studentSearch").value = "";
        document.getElementById("selectedStudent").style.display = "none";
        selectedStudentId = null;

    } catch (err) {
        alert("X" + err.message);
    }
}