const btnSignIn = document.getElementById("signIn"); 
const toast  = document.getElementById("toast");
const API_URL = "https://localhost:7223/api/Users/SignIn";

const username = document.getElementById("username");
const password = document.getElementById("password");
const rememberMeCheck = document.getElementById("rememberme");

window.addEventListener("load", () => {
    let usernameValue = localStorage.getItem("UserName");
    let passwordValue = localStorage.getItem("password");

    if(usernameValue !== "" || passwordValue !== "")
    {
        username.value = usernameValue;
        password.value = passwordValue;
    }
});

const rules = {
    username: {
        required: true,
        pattern:  null,
        message:  ""
    },
    password: {
        required: true,
        pattern:  /^.{2,}$/,          // [TODO] Make the password validation more strong.
        message:  "Password must be at least 6 characters."
    },
};
function buildPayload() {
    return {
        username: document.getElementById("username").value.trim(),
        password: document.getElementById("password").value.trim(),
    };
}
function validateField(id) {
    const rule  = rules[id];
    if (!rule) return true;

    const el    = document.getElementById(id);
    if (!el) return true;

    const errEl = document.getElementById(`err-${id}`);
    const val   = el.value.trim();

    let error = "";

    if (rule.required && val === "") {
        error = "This field is required.";
    } else if (rule.pattern && val !== "" && !rule.pattern.test(val)) {
        error = rule.message;
    }

    if (error) {
        el.classList.add("invalid");
        el.classList.remove("valid");
        if (errEl) errEl.textContent = error;
        return false;
    } else {
        el.classList.remove("invalid");
        if (val !== "") el.classList.add("valid");
        if (errEl) errEl.textContent = "";
        return true;
    }
}
Object.keys(rules).forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;
    el.addEventListener("blur",  () => validateField(id));
    el.addEventListener("input", () => {
        if (el.classList.contains("invalid")) validateField(id);
    });
});
function validateAll() {
    return Object.keys(rules)
        .map(id => validateField(id))
        .every(Boolean);
}

btnSignIn.addEventListener("click", async () => {
    if (!validateAll()) {
        showToast("Please fix the errors before signing in.", "error");
        return;
    }

    try {
        const payload = buildPayload();

        let response;

        response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });

        if (!response) {
            throw new Error("No response from server");
        }

        if (!response.ok) {
            let errorMessage = `Server error (${response.status})`;

            try {
                const errBody = await response.json();
                errorMessage = errBody.message
                    || errBody.title
                    || JSON.stringify(errBody.errors ?? errBody)
                    || errorMessage;
            } catch {
                errorMessage = response.statusText || errorMessage;
            }

            throw new Error(errorMessage);
        }

        let username = document.getElementById("username");
        let usernameValue = username.value;
        let passwordValue = password.value;
        let isCheck = rememberMeCheck.checked;
        
        if(isCheck)
        {
            localStorage.setItem("UserName", usernameValue);
            localStorage.setItem("password", passwordValue);
        }
        
        showToast("Login done successfully!", "success");

        await fetchAndSaveImagePath(usernameValue);
        await fetchAndSaveUserID(usernameValue);
        await checkIfUserIsActive(usernameValue);
        

        setTimeout(() => {
            window.location.href = "/main.html";
        }, 1400);

    } catch (error) {
        console.error("Error:", error);
        showToast("Invalid credentials or account is inactive.", "error");
    }
});


// ── Toast helper ──────────────────────────────
let toastTimer;
function showToast(message, type = "success") {
    clearTimeout(toastTimer);
    toast.textContent = message;
    toast.className   = `toast ${type} show`;
    toastTimer = setTimeout(() => {
        toast.classList.remove("show");
    }, 3000);
}


async function fetchAndSaveImagePath(usernameValue) {
    try {
        const res = await fetch(`https://localhost:7223/api/Users/ImagePath?Username=${encodeURIComponent(usernameValue)}`);
        if (!res.ok) return;
        const imagePath = await res.text();
        localStorage.setItem("ImagePath", imagePath);
    } catch (err) {
        console.error("Failed to fetch image path:", err);
    }
}
async function fetchAndSaveUserID(usernameValue) {
    try {
        const res = await fetch(`https://localhost:7223/api/Users/UserID?Username=${encodeURIComponent(usernameValue)}`);
        if (!res.ok) return;
        const UserID = await res.text();
        localStorage.setItem("UserID", UserID);
    } catch (err) {
        console.error("Failed to fetch UserID :", err);
    }
}
async function checkIfUserIsActive(usernameValue) {
    try {
        const res = await fetch(`https://localhost:7223/api/Users/Active?Username=${encodeURIComponent(usernameValue)}`);
        if (!res.ok) return false;
        const isActive = await res.text();
        const isActiveText = isActive == 1 ? "true" : "false";
        localStorage.setItem("isActive", isActiveText);
    } catch (err) {
        console.error("Failed to check user active status:", err);
        return false;
    }
}