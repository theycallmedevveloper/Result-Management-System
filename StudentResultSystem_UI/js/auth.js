// Handle user login with role verification
async function login(event) {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const selectedRole = document.getElementById("role").value;
    const msgEl = document.getElementById("msg");

    msgEl.classList.remove("show");

    if (!selectedRole) {
        msgEl.innerText = "Please select your role to continue";
        msgEl.classList.add("show");
        return;
    }

    try {
        // Attempt login
        const res = await fetch("https://localhost:7240/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ username, password })
        });

        if (!res.ok) {
            msgEl.innerText = "Invalid username or password";
            msgEl.classList.add("show");
            return;
        }

        // Get user profile to verify role
        const profileRes = await fetch("https://localhost:7240/api/auth/profile", {
            credentials: "include"
        });

        const profile = await profileRes.json();

        // Check if selected role matches actual role
        if (profile.role !== selectedRole) {
            await fetch("https://localhost:7240/api/auth/logout", {
                method: "POST",
                credentials: "include"
            });

            msgEl.innerText = "Role mismatch. Please select the correct role for your account.";
            msgEl.classList.add("show");
            return;
        }

        // Redirect based on role
        if (profile.role === "Admin") {
            localStorage.setItem("role", "Admin");
            window.location.href = "admin.html";
        } else {
            // Store student info for student dashboard
            localStorage.setItem("role", "Student");
            localStorage.setItem("studentId", profile.studentId);

            window.location.href = "student.html";
        }

    } catch (error) {
        msgEl.innerText = "Connection error. Please try again.";
        msgEl.classList.add("show");
    }
}