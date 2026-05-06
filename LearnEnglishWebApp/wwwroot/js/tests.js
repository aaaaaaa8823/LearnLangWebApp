let allTests = [];

async function loadTests() {
    const container = document.getElementById('testsContainer');
    if (!container) return;

    try {
        const response = await fetch('/api/Grammar/tests', {
            credentials: 'include'
        });

        if (response.status === 401) {
            console.log('Not authorized');
            return;
        }

        if (!response.ok) {
            throw new Error('Ошибка загрузки тестов');
        }

        allTests = await response.json();
        console.log('Загружено тестов:', allTests.length);
        displayTests(allTests);

    } catch (error) {
        console.error('Ошибка:', error);
        container.innerHTML = '<div class="error">Ошибка загрузки тестов</div>';
    }
}

function displayTests(tests) {
    const container = document.getElementById('testsContainer');
    if (!tests || tests.length === 0) {
        console.log('Тесты не найдены')
        container.innerHTML = '<div class="empty">Тесты не найдены</div>';
        return;
    }

    container.innerHTML = tests.map(test => {
        let cardClass = '';
        let statusBadge = '';

        if (test.isCompleted) {
            cardClass = 'completed';
            statusBadge = '<span class="completed-badge">✓ Пройден</span>';
        }

        return `
            <div class="test-card ${cardClass}" onclick="startTest(${test.id})">
                <div class="test-header">
                    <span class="test-level">${escapeHtml(test.level)}</span>
                    <span class="test-type grammar">Грамматика</span>
                    ${statusBadge}
                </div>
                <div class="test-body">
                    <div class="test-title">${escapeHtml(test.title)}</div>
                    <div class="test-description">
                        ${test.questionCount} вопросов |
                        Проходной балл: ${test.passingScore}% |
                        Время: ${test.timeLimitMinutes} мин
                    </div>
                </div>
                <div class="test-footer">
                    <div class="test-stats">
                        ${test.attemptCount > 0 ? `
                            <span class="attempts">Попыток: ${test.attemptCount}</span>
                            ${test.bestPercentage ? `<span class="best-score">Лучший: ${Math.round(test.bestPercentage)}%</span>` : ''}
                        ` : `
                            <span>Еще не начат</span>
                        `}
                    </div>
                    <button class="btn-start" onclick="event.stopPropagation(); startTest(${test.id})">
                        ${test.isCompleted ? 'Пройти заново' : 'Начать тест'}
                    </button>
                </div>
            </div>
        `;
    }).join('');
}

function searchTests() {
    const searchTerm = document.getElementById('testSearch').value.toLowerCase();

    if (!searchTerm) {
        displayTests(allTests);
        return;
    }

    const filtered = allTests.filter(test =>
        test.title.toLowerCase().includes(searchTerm)
    );
    displayTests(filtered);
}

function startTest(testId) {
    window.location.href = `/Home/TestDetail?id=${testId}&type=grammar`;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

let searchTimeout;
function handleSearchInput() {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(searchTests, 300);
}

document.addEventListener('DOMContentLoaded', () => {
    loadTests();

    const searchInput = document.getElementById('testSearch');
    if (searchInput) {
        searchInput.addEventListener('input', handleSearchInput);
    }
});


