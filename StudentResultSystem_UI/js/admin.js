let selectedStudentId = null;

async function addMarks() {
    if (!selectedStudentId) {
        alert("⚠️ Please select a student from suggestions");
        return;
    }

    const subjectId = document.getElementById("subjectSelect").value;
    const marks = document.getElementById("marks").value;

    if (!subjectId || !marks) {
        alert("⚠️ Please fill in all fields");
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
            alert("❌ Failed to add marks");
            return;
        }

        alert("✅ Marks added successfully");

        // Clear form
        document.getElementById("studentSearch").value = "";
        document.getElementById("subjectSelect").value = "";
        document.getElementById("marks").value = "";
        document.getElementById("selectedStudent").style.display = "none";
        selectedStudentId = null;

    } catch (error) {
        alert("❌ Error: " + error.message);
    }
}

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

function selectStudent(student) {
    selectedStudentId = student.studentId;
    document.getElementById("studentSearch").value =
        `${student.firstName} ${student.lastName}`;

    const selectedDiv = document.getElementById("selectedStudent");
    selectedDiv.innerText = `✓ Selected: ${student.firstName} ${student.lastName}`;
    selectedDiv.style.display = "block";

    document.getElementById("suggestions").innerHTML = "";
    const firstSelect = document.querySelector(".subjectSelect");
    loadSubjects(firstSelect);
}

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

function removeSubjectRow(button) {
    const row = button.closest(".mark-row");
    row.remove();

    // Re-enable subjects after removal
    handleSubjectChange();
}

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

        // Group results by student
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

        // Render grouped results
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
                ❌  
            </button>
        </div>
    `;

    container.appendChild(row);

    //  Load subjects ONLY for the new row
    const newSelect = row.querySelector(".subjectSelect");
    loadSubjects(newSelect);
}

async function addMultipleMarks() {
    if (!selectedStudentId) {
        alert("⚠️ Please select a student");
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
        alert("⚠️ Please enter at least one subject & marks");
        return;
    }

    try {
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

        alert("✅ All marks added successfully");

        // Reset UI
        document.getElementById("marksContainer").innerHTML = "";
        addMoreSubjectRow();
        document.getElementById("studentSearch").value = "";
        document.getElementById("selectedStudent").style.display = "none";
        selectedStudentId = null;

    } catch (err) {
        alert("❌ " + err.message);
    }
}