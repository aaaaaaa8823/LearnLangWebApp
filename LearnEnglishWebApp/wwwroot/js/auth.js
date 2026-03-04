document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM загружен, инициализация обработчиков...');
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
}

const apiUrl = '/api';
let authToken = localStorage.getItem('token');

function checkAuthStatus() {
    const token = localStorage.getItem('token'); 
    console.log('Проверка токена:', token);

    if (token && !isTokenExpired(token)) {
        console.log('Токен валидный, редирект на профиль');
        window.location.href = '/Home/Me';
    } else {
        console.log('Токен не найден или истек');
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

function switchTab(tab) {
    console.log('Переключение на вкладку:', tab);

    document.querySelectorAll('.tab-btn').forEach(btn => {
        if (btn.dataset.tab === tab) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });

    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');

    if (loginForm && registerForm) {
        if (tab === 'login') {
            loginForm.classList.add('active');
            registerForm.classList.remove('active');
        } else {
            registerForm.classList.add('active');
            loginForm.classList.remove('active');
        }
    }
}

async function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/';
        throw new Error('Нет токена авторизации');
    }

    if (isTokenExpired(token)) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Токен истек');
    }

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };

    const response = await fetch(url, {
        ...options,
        headers
    });

    if (response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Не авторизован');
    }
    return response;
}

async function register() {
    const username = document.getElementById('register-username').value;
    const email = document.getElementById('register-email').value;
    const password = document.getElementById('register-password').value;
    const confirm = document.getElementById('register-confirm').value;
    const errorDiv = document.getElementById('register-error');

    if (!username || !email || !password || !confirm) {
        errorDiv.textContent = 'Все поля обязательны для заполнения';
        return;
    }

    if (password !== confirm) {
        errorDiv.textContent = 'Проверьте правильность пароля';
        return;
    }

    if (password.length < 6) {
        errorDiv.textContent = 'Пароль должен быть не менее 6 символов';
        return;
    }

    try {
        const response = await fetch(`${API_URL}/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Ошибка регистрации');
        }

        if (response.ok) {
            document.getElementById('login-email').value = email;
            document.getElementById('login-password').value = password;
            switchTab('login'); 
            login(); 
        }
    } catch (error) {
        errorDiv.textContent = error.message;
    }
}

async function login() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const errorDiv = document.getElementById('login-error');

    if (!email || !password) {
        errorDiv.textContent = 'Введите email и пароль';
        return;
    }

    try {
        const response = await fetch(`${API_URL}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Ошибка входа. Проверьте правильность заполнения полей');
        }

        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        window.location.href = '/Home/Me';
    } catch (error) {
        errorDiv.textContent = error.message;
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/';
}
