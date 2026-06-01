async function loadComponent(selector, url) {
    const container = document.querySelector(selector);
    if (!container) return;

    const response = await fetch(url);
    container.innerHTML = await response.text();
}

async function initLayout() {
    await loadComponent("#sidebar", "/components/sidebar.txt");
    await loadComponent("#pagination", "/components/Pagination.txt");
    await loadComponent("#toast", "/components/Toast.txt");

    // أطلق event بعد ما يكتمل تحميل كل الـ components
    document.dispatchEvent(new Event("layoutReady"));
    setActiveNavLink();
}

initLayout();

function setActiveNavLink() {
    const currentPath = window.ACTIVE_PAGE || window.location.pathname;

    // ── Clear all active states first ─────────────────────────
    document.querySelectorAll('.nav a, .nav-dropdown-toggle, .nav-sub-dropdown-toggle')
        .forEach(el => el.classList.remove('active'));

    // ── Mark exact link as active ──────────────────────────────
    document.querySelectorAll('a[href]').forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });

// ── Mark parent dropdown toggles as active ─────────────────
    if (currentPath.includes('People')) {
        document.querySelector('a')
            ?.classList.add('active');
    }

    // ── Mark parent dropdown toggles as active ─────────────────
    if (currentPath.includes('Applications') || currentPath.includes('DetainRelse')) {
        document.querySelector('#applicationsDropdown .nav-dropdown-toggle')
            ?.classList.add('active');
        document.getElementById('applicationsDropdown')
            ?.classList.add('open');
    }

    if (currentPath.includes('/Sections/AccountSettings')) {
    document.querySelector('#accountSettingsDropdown .nav-dropdown-toggle')
        ?.classList.add('active');
    document.getElementById('accountSettingsDropdown')
        ?.classList.add('open');
    }
}