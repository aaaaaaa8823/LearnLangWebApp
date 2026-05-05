const APP_API_URL = '/api';
console.log('APP_API_URL:', APP_API_URL);

document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM загружен, инициализация обработчиков. мяу...');
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

    if (tab === 'login') {
        subtitle.textContent = 'Продолжи учить иностранные языки сегодня!';
    } else {
        subtitle.textContent = 'Начни учить иностранные языки сегодня!';
    }
}


function isTokenExpired(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const exp = payload.exp * 1000;
        return Date.now() >= exp;
    } catch (e) {
        return true;
    }
}

async function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/';
        throw new Error('Нет токена авторизации');
    }
    if (isTokenExpired(token)) {
        logout(); 
        throw new Error('Токен истек');
    }
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };
    const response = await fetch(url, { ...options, headers });
    if (response.status === 401) {
        logout();
        throw new Error('Не авторизован');
    }
    return response;
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
        console.log('Ответ регистрации:', data);

        if (!response.ok) {
            if (data.errors) {
                const errorMessages = [];
                for (const field in data.errors) {
                    errorMessages.push(`${field}: ${data.errors[field].join(', ')}`);
                }
                throw new Error(errorMessages.join('; '));
            }
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
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Неверный email или пароль');
        }

        if (!data.token || !data.user) {
            throw new Error('Неверный формат ответа от сервера');
        }

        localStorage.setItem('user', JSON.stringify(data.user));

        if (data.user.role) {
            localStorage.setItem('userRole', data.user.role);
        }

        if (data.user.role === 'Administrator') {
            window.location.href = '/Admin';
        } else {
            window.location.href = '/Home/Me';
        }

    } catch (error) {
        console.error('Ошибка входа:', error);
        errorDiv.textContent = error.message;
    }
}

function logout() {
    console.log('Выход из системы...');
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('userRole');

    document.cookie = 'auth_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';

    window.location.href = '/';
}