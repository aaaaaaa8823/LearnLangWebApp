function logout() {
    console.log('Выход из системы...');
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/';
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function redirectAdmin(){
    const userRole = localStorage.getItem('userRole');
    const currentPath = window.location.pathname;

    if (userRole === 'Administrator' && currentPath === '/Home/Me') {
        console.log('Админ на странице Me, перенаправляем на /Admin');
        window.location.href = '/Admin';
        return true;
    }
    return false;
}
async function loadSidebarUser() {
    const userNameElement = document.getElementById('userName');
    const userInitialsElement = document.getElementById('userInitials');
    const userLevelElement = document.getElementById('userLevel');

    if (!userNameElement) return;

    try {
        let user = localStorage.getItem('user');

        if (user) {
            user = JSON.parse(user);
        } else {
            const response = await fetch('/api/Profile/me');

            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('user');
                    localStorage.removeItem('userRole');
                    window.location.href = '/';
                }
                throw new Error('Ошибка загрузки');
            }

            user = await response.json();
            localStorage.setItem('user', JSON.stringify(user));

            if (user.role) {
                localStorage.setItem('userRole', user.role);
            }
        }

        userNameElement.textContent = user.userName || user.username;
        userInitialsElement.textContent = (user.userName || user.username)[0].toUpperCase();
        userLevelElement.textContent = user.level || 'A1';

        const adminNavItem = document.getElementById('adminNavItem');
        if (adminNavItem) {
            const userRole = user.role || localStorage.getItem('userRole');
            if (userRole === 'Administrator') {
                adminNavItem.style.display = 'block';
            } else {
                adminNavItem.style.display = 'none';
            }
        }

        redirectAdmin();

    } catch (error) {
        console.error('Ошибка загрузки пользователя:', error);
        userNameElement.textContent = 'Ошибка';
    }
}

async function loadAccountPage() {
    const container = document.getElementById('profileContainer');
    if (!container) {
        console.log('Контейнер profileContainer не найден');
        return;
    }

    console.log('Загружаем страницу аккаунта...');

    try {
        let user = localStorage.getItem('user');

        if (!user) {
            const response = await fetch('/api/Profile/me');
            if (!token) {
                console.log('Нет токена, редирект');
                window.location.href = '/';
                return;
            }

            console.log('Запрашиваем данные пользователя...');
            const response = await fetch('/api/Profile/me', {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (!response.ok) {
                throw new Error(`Ошибка: ${response.status}`);
            }

            user = await response.json();
            console.log('Получены данные:', user);
            localStorage.setItem('user', JSON.stringify(user));
        } else {
            user = JSON.parse(user);
            console.log('Данные из localStorage:', user);
        }


        container.innerHTML = `
            <h1>Настройки аккаунта</h1>
            
            <div class="account-info">
                <form id="profileForm" onsubmit="updateProfile(event)">
                    <div class="form-group">
                        <label for="username">Имя пользователя</label>
                        <input type="text" id="username" name="username" 
                               value="${escapeHtml(user.userName || user.username)}" 
                               class="form-input" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="email">Email</label>
                        <input type="email" id="email" name="email" 
                               value="${escapeHtml(user.email)}" 
                               class="form-input" required>
                    </div>
                    
                    <div class="form-group">
                        <label for="level">Уровень владения языком</label>
                        <select id="level" name="level" class="form-select">
                            <option value="A1" ${user.level === 'A1' ? 'selected' : ''}>A1 - Начальный</option>
                            <option value="A2" ${user.level === 'A2' ? 'selected' : ''}>A2 - Элементарный</option>
                            <option value="B1" ${user.level === 'B1' ? 'selected' : ''}>B1 - Средний</option>
                            <option value="B2" ${user.level === 'B2' ? 'selected' : ''}>B2 - Выше среднего</option>
                            <option value="C1" ${user.level === 'C1' ? 'selected' : ''}>C1 - Продвинутый</option>
                            <option value="C2" ${user.level === 'C2' ? 'selected' : ''}>C2 - Профессиональный</option>
                        </select>
                    </div>
                    
                    <div class="form-divider">
                        <h3>Смена пароля</h3>
                        <p class="form-hint">Оставьте поля пустыми, если не хотите менять пароль</p>
                    </div>
                    
                    <div class="form-group">
                        <label for="currentPassword">Текущий пароль</label>
                        <input type="password" id="currentPassword" name="currentPassword" 
                               class="form-input" placeholder="Введите текущий пароль">
                    </div>
                    
                    <div class="form-group">
                        <label for="newPassword">Новый пароль</label>
                        <input type="password" id="newPassword" name="newPassword" 
                               class="form-input" placeholder="Введите новый пароль">
                    </div>
                    
                    <div class="form-group">
                        <label for="confirmPassword">Подтверждение пароля</label>
                        <input type="password" id="confirmPassword" name="confirmPassword" 
                               class="form-input" placeholder="Подтвердите новый пароль">
                    </div>
                    
                    <div id="formMessage" class="form-message"></div>
                    
                    <div class="form-actions">
                        <button type="submit" class="btn-primary">Сохранить изменения</button>
                        <button type="button" onclick="cancelEdit()" class="btn-secondary">Отмена</button>
                    </div>
                </form>
            </div>
        `;

    } catch (error) {
        console.error('Ошибка загрузки страницы аккаунта:', error);
        container.innerHTML = `<div class="error">Ошибка загрузки данных: ${error.message}</div>`;
    }
}

async function updateProfile(event) {
    event.preventDefault();

    console.log('Обновляем профиль...');

    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const level = document.getElementById('level').value;
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const messageDiv = document.getElementById('formMessage');

    if (!username || !email) {
        showMessage('Заполните все обязательные поля', 'error');
        return;
    }

    if (newPassword && newPassword !== confirmPassword) {
        showMessage('Новые пароли не совпадают', 'error');
        return;
    }

    const updateData = {
        userName: username,
        email: email,
        level: level
    };

    if (newPassword) {
        updateData.currentPassword = currentPassword;
        updateData.newPassword = newPassword;
    }

    try {
        const response = await fetch('/api/Profile/update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updateData)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка обновления');
        }

        const updatedUser = await response.json();
        console.log('Обновленные данные:', updatedUser);

        localStorage.setItem('user', JSON.stringify(updatedUser));
        if (updatedUser.role) {
            localStorage.setItem('userRole', updatedUser.role);
        }

        await loadSidebarUser();
        showMessage('Профиль успешно обновлен!', 'success');

        document.getElementById('currentPassword').value = '';
        document.getElementById('newPassword').value = '';
        document.getElementById('confirmPassword').value = '';

        setTimeout(() => {
            location.reload();
        }, 1500);

    } catch (error) {
        console.error('Ошибка обновления:', error);
        showMessage(error.message, 'error');
    }
}

function showMessage(text, type) {
    const messageDiv = document.getElementById('formMessage');
    if (messageDiv) {
        messageDiv.textContent = text;
        messageDiv.className = `form-message ${type}`;
        setTimeout(() => {
            messageDiv.textContent = '';
            messageDiv.className = 'form-message';
        }, 3000);
    }
}

function cancelEdit() {
    loadAccountPage();
}
