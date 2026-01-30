// Load student's own results
async function loadMyResult() {
    try {
        const res = await fetch("https://localhost:7240/api/marks/my-result", {
            credentials: "include"
        });

        if (!res.ok) {
            alert("Failed to load results. Please try again.");
            return;
        }

        const data = await res.json();
        const tbody = document.getElementById("myResult");

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

        let totalMarks = 0;
        let isPassed = true;

        // Calculate totals and check pass/fail status
        data.forEach(r => {
            totalMarks += r.marksObtained;

            if (r.marksObtained < 35) {
                isPassed = false;
            }

            tbody.innerHTML += `
                <tr>
                    <td>${r.subjectName}</td>
                    <td>${r.marksObtained}</td>
                    <td>${r.maxMarks}</td>
                </tr>
            `;
        });

        const percentage = ((totalMarks / (data.length * 100)) * 100).toFixed(2);

        // Update summary section
        document.getElementById("totalMarks").textContent = totalMarks;
        document.getElementById("percentage").textContent = percentage + "%";

        const statusEl = document.getElementById("resultStatus");
        statusEl.textContent = isPassed ? "PASSED" : "FAILED";
        statusEl.style.color = isPassed ? "green" : "red";

    } catch (error) {
        alert("Error loading results: " + error.message);
    }
}

// Open profile modal with student details
async function openProfile() {
    try {
        const studentId = localStorage.getItem("studentId");

        if (!studentId) {
            alert("Student ID not found. Please login again.");
            return;
        }

        const res = await fetch(
            `https://localhost:7240/api/students/${studentId}/profile`,
            {
                credentials: "include"
            }
        );

        if (!res.ok) {
            throw new Error("Profile API failed");
        }

        const data = await res.json();

        // Populate profile modal with data
        document.getElementById("profilePhoto").src =
            "https://localhost:7240" + data.profilePhotoUrl;

        document.getElementById("pName").textContent =
            `${data.firstName} ${data.lastName}`;
        document.getElementById("pRoll").textContent = data.rollNumber;
        document.getElementById("pClass").textContent = data.class;
        document.getElementById("pEmail").textContent = data.email;

        // Show the modal
        const modal = new bootstrap.Modal(
            document.getElementById("profileModal")
        );
        modal.show();

    } catch (err) {
        console.error(err);
        alert("Unable to load profile details");
    }
}

// Upload new profile photo
async function uploadProfilePhoto() {
    const fileInput = document.getElementById("photoInput");
    const file = fileInput.files[0];

    if (!file) return;

    const formData = new FormData();
    formData.append("photo", file);

    try {
        const res = await fetch(
            "https://localhost:7240/api/students/upload-profile-photo",
            {
                method: "POST",
                credentials: "include",
                body: formData
            }
        );

        if (!res.ok) {
            alert("Failed to upload photo");
            return;
        }

        const data = await res.json();

        // Update photo with cache busting
        document.getElementById("profilePhoto").src =
            "https://localhost:7240" + data.profilePhotoUrl + "?t=" + Date.now();

        alert("Profile photo updated successfully");

    } catch (err) {
        alert("Upload error");
        console.error(err);
    }
}

// Logout student user
async function logout() {
    try {
        await fetch("https://localhost:7240/api/auth/logout", {
            method: "POST",
            credentials: "include"
        });
        window.location.href = "index.html";
    } catch {
        window.location.href = "index.html";
    }
}