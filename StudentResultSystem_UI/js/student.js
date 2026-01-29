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
        const totalPercentageDiv = document.getElementById("totalPercentage");
        const percentageValue = document.getElementById("percentageValue");
        
        tbody.innerHTML = "";

        if (data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="text-center text-muted">
                        No results found
                    </td>
                </tr>
            `;
            totalPercentageDiv.style.display = "none";
            return;
        }

        let totalMarksObtained = 0;
        let totalMaxMarks = 0;

        data.forEach(r => {
            totalMarksObtained += r.marksObtained;
            totalMaxMarks += r.maxMarks;
            
            tbody.innerHTML += `
                <tr>
                    <td>${r.subjectName}</td>   
                    <td>${r.marksObtained}</td>
                    <td>${r.maxMarks}</td>
                </tr>
            `;
        });

        const overallPercentage = ((totalMarksObtained / totalMaxMarks) * 100).toFixed(2);
        percentageValue.textContent = `${overallPercentage}%`;
        totalPercentageDiv.style.display = "block";
        
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