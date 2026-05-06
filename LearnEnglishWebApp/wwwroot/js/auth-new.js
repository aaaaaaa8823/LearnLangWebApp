const APP_API_URL = '/api';
console.log('APP_API_URL:', APP_API_URL);

document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM загружен, инициализация обработчиков');
    initTabs();
});

function initTabs() {
    const tabBtns = document.querySelectorAll('.tab-btn');
    tabBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            const tab = this.dataset.tab;
            switchTab(tab);
        });
    });
}

function switchTab(tab) {
    console.log('Переключение на вкладку:', tab);
    document.querySelectorAll('.tab-btn').forEach(btn => {
        if (btn.dataset.tab === tab) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });
    document.querySelectorAll('.auth-form').forEach(form => {
        if (form.id === tab + '-form') {
            form.classList.add('active');
        } else {
            form.classList.remove('active');
        }
    });

    const subtitle = document.getElementById('authSubtitle');
    if (subtitle) {
        if (tab === 'login') {
            subtitle.textContent = 'Продолжи учить иностранные языки сегодня!';
        } else {
            subtitle.textContent = 'Начни учить иностранные языки сегодня!';
        }
    }
}

async function register() {
    const username = document.getElementById('register-username').value.trim();
    const email = document.getElementById('register-email').value.trim();
    const password = document.getElementById('register-password').value;
    const confirm = document.getElementById('register-confirm').value;
    const errorDiv = document.getElementById('register-error');

    errorDiv.textContent = '';

    if (!username || !email || !password || !confirm) {
        errorDiv.textContent = 'Все поля обязательны для заполнения';
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        errorDiv.textContent = 'Введите корректный email адрес';
        return;
    }

    if (password !== confirm) {
        errorDiv.textContent = 'Пароли не совпадают';
        return;
    }

    if (password.length < 6) {
        errorDiv.textContent = 'Пароль должен быть не менее 6 символов';
        return;
    }

    try {
        console.log('Регистрация с данными:', { username, email });

        const response = await fetch(`${APP_API_URL}/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ username, email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Ошибка регистрации');
        }

        console.log('Регистрация успешна!');

        document.getElementById('register-username').value = '';
        document.getElementById('register-email').value = '';
        document.getElementById('register-password').value = '';
        document.getElementById('register-confirm').value = '';

        switchTab('login');

        const loginErrorDiv = document.getElementById('login-error');
        if (loginErrorDiv) {
            loginErrorDiv.textContent = 'Регистрация успешна! Теперь войдите в аккаунт.';
            loginErrorDiv.style.color = 'green';
        }

    } catch (error) {
        console.error('Ошибка регистрации:', error);
        errorDiv.textContent = error.message;
    }
}

async function login() {
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');

    errorDiv.textContent = '';

    if (!email || !password) {
        errorDiv.textContent = 'Введите email и пароль';
        return;
    }

    try {
        const response = await fetch(`${APP_API_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
            credentials: 'include'
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Неверный email или пароль');
        }

        console.log('Login response:', data);
        console.log('Redirecting to:', data.redirectUrl);

        if (data.user) {
            localStorage.setItem('user', JSON.stringify(data.user));
            localStorage.setItem('userRole', data.user.role);
        }

        setTimeout(() => {
            window.location.href = data.redirectUrl || '/Home/Me';
        }, 100);

    } catch (error) {
        errorDiv.textContent = error.message;
    }
}

async function logout() {
    console.log('Выход из системы...');

    try {
        await fetch(`${APP_API_URL}/Auth/logout`, {
            method: 'POST',
            credentials: 'include'
        });
    } catch (error) {
        console.error('Ошибка при выходе:', error);
    }

    localStorage.removeItem('user');
    localStorage.removeItem('userRole');
    window.location.href = '/';
}

async function checkAuthStatus() {
    try {
        const response = await fetch('/api/Profile/me', {
            credentials: 'include'
        });

        if (response.ok) {
            const user = await response.json();
            if (user.role === 'Administrator') {
                window.location.href = '/Admin';
            } else {
                window.location.href = '/Home/Me';
            }
        }
    } catch (error) {
        console.log('Не авторизован');
    }
}