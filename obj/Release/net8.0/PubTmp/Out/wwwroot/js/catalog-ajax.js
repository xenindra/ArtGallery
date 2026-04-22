

let currentPage = 1;
let searchTimer = null;
let yearTimer   = null;

function getFilters() {
    return {
        search: document.getElementById('search-input')?.value.trim() ?? '',
        styles: [...document.querySelectorAll('input[data-filter="style"]:checked')].map(el => el.value),
        countries: [...document.querySelectorAll('input[data-filter="country"]:checked')].map(el => el.value),
        yearFrom: document.getElementById('year-from')?.value ?? '',
        yearTo: document.getElementById('year-to')?.value ?? '',
        sort: document.getElementById('sort-select')?.value ?? 'name-asc',
    };
}

function buildParams(filters, page) {
    const parts = [];
    if (filters.search)   parts.push('search='   + encodeURIComponent(filters.search));
    if (filters.yearFrom) parts.push('yearFrom=' + encodeURIComponent(filters.yearFrom));
    if (filters.yearTo)   parts.push('yearTo='   + encodeURIComponent(filters.yearTo));
    if (filters.sort)     parts.push('sort='     + encodeURIComponent(filters.sort));
    if (page)             parts.push('page='     + page);
    filters.styles.forEach(s    => parts.push('styles='    + encodeURIComponent(s)));
    filters.countries.forEach(c => parts.push('countries=' + encodeURIComponent(c)));
    return parts.join('&');
}

// ── Сценарий 1: умный поиск ──────────────────────────────────────
const searchInput = document.getElementById('search-input');
const suggestionsBox = document.getElementById('search-suggestions');

if (searchInput && suggestionsBox) {
    searchInput.addEventListener('input', function () {
        const q = this.value.trim();
        clearTimeout(searchTimer);
        suggestionsBox.innerHTML = '';
        suggestionsBox.style.display = 'none';

        if (q.length < 1) {
            updateGrid(1);
            return;
        }

        searchTimer = setTimeout(() => {
            fetch('/Catalog/Search?q=' + encodeURIComponent(q))
                .then(r => r.json())
                .then(items => {
                    if (!items.length) {
                        suggestionsBox.style.display = 'none';
                        return;
                    }

                    suggestionsBox.innerHTML = '';
                    items.forEach(item => {
                        const div = document.createElement('div');
                        div.className = 'suggestion-item';
                        div.textContent = item.label;

                        // При клике на подсказку
                        div.addEventListener('click', function () {
                            // Вставляем полное название в строку поиска
                            searchInput.value = item.title;
                            // Скрываем подсказки
                            suggestionsBox.style.display = 'none';
                            // Обновляем каталог с новым поиском
                            currentPage = 1;
                            updateGrid(1);
                        });

                        suggestionsBox.appendChild(div);
                    });

                    suggestionsBox.style.display = 'block';
                });
        }, 300);
    });

    // Скрываем подсказки при клике вне
    document.addEventListener('click', e => {
        if (!searchInput.contains(e.target) && !suggestionsBox.contains(e.target))
            suggestionsBox.style.display = 'none';
    });
}

// ── Сценарий 2: модальная карточка ──────────────────────────────
function openInfoModal(id) {
    const modal   = document.getElementById('info-modal');
    const content = document.getElementById('modal-content');
    if (!modal || !content) return;
    content.innerHTML = '<p class="modal-loading">Загрузка...</p>';
    modal.style.display = 'flex';

    fetch('/Catalog/PaintingInfo/' + id)
        .then(r => r.json())
        .then(p => {
            content.innerHTML =
                '<button class="modal-close" onclick="closeInfoModal()">&#x00D7;</button>' +
                '<h2 class="modal-title">' + escapeHtml(p.title) + '</h2>' +
                '<div class="modal-fields">' +
                  '<div class="modal-row"><span>Автор</span><strong>'     + escapeHtml(p.author)    + '</strong></div>' +
                  '<div class="modal-row"><span>Год</span><strong>'       + p.year                  + '</strong></div>' +
                  '<div class="modal-row"><span>Стиль</span><strong>'     + escapeHtml(p.style)     + '</strong></div>' +
                  '<div class="modal-row"><span>Материалы</span><strong>' + escapeHtml(p.materials) + '</strong></div>' +
                  '<div class="modal-row"><span>Размер</span><strong>'    + escapeHtml(p.size)      + '</strong></div>' +
                  '<div class="modal-row"><span>Страна</span><strong>'    + escapeHtml(p.country)   + '</strong></div>' +
                '</div>' +
                '<p class="modal-description">' + escapeHtml(p.description) + '</p>' +
                '<div style="margin-top:1rem;display:flex;gap:1rem;flex-wrap:wrap;">' +
                  '<a href="/Paintings/' + p.id + '" class="modal-btn-primary">Подробнее →</a>' +
                  '<button class="add-to-favorites ' + (p.isFavorite ? 'in-favorites' : '') + '"' +
                          ' onclick="toggleFavorite(' + p.id + ', this)">' +
                    (p.isFavorite ? 'Убрать из избранного' : 'Добавить в избранное') +
                  '</button>' +
                '</div>';
        });
}

function closeInfoModal() {
    const m = document.getElementById('info-modal');
    if (m) m.style.display = 'none';
}
document.getElementById('info-modal')?.addEventListener('click', function(e) {
    if (e.target === this) closeInfoModal();
});

// ── Сценарий 3: избранное ────────────────────────────────────────
function toggleFavorite(id, btn) {
    fetch('/Catalog/ToggleFavorite', {
        method:  'POST',
        headers: { 'Content-Type': 'application/json' },
        body:    JSON.stringify({ paintingId: id })
    })
    .then(r => r.json())
    .then(data => {
        const added = data.status === 'added';
        const label = added ? 'Убрать из избранного' : 'Добавить в избранное';
        document.querySelectorAll('.fav-btn-' + id).forEach(b => {
            b.textContent = label;
            b.classList.toggle('in-favorites', added);
        });
        if (btn) { btn.textContent = label; btn.classList.toggle('in-favorites', added); }
        showNotification(data.message, added ? 'success' : 'info');
    });
}

// ── Сценарий 4: живой счётчик ────────────────────────────────────
function updateCounter() {
    fetch('/Catalog/Count?' + buildParams(getFilters(), null))
        .then(r => r.json())
        .then(data => {
            const el = document.getElementById('results-count');
            if (el) el.textContent = 'Найдено ' + data.count + ' произведений';
        });
}

// ── ЛР4: обновление сетки ────────────────────────────────────────
function updateGrid(page) {
    currentPage = page || currentPage;
    const content = document.getElementById('catalog-content');
    if (!content) return;
    content.style.opacity = '0.4';
    content.style.pointerEvents = 'none';

    fetch('/Catalog/Grid?' + buildParams(getFilters(), currentPage))
        .then(r => r.text())
        .then(html => {
            content.innerHTML = html;
            content.style.opacity = '1';
            content.style.pointerEvents = '';
            updateCounter();
        })
        .catch(() => { content.style.opacity = '1'; content.style.pointerEvents = ''; });
}

function loadPage(page) {
    currentPage = page;
    updateGrid(page);
    window.scrollTo({ top: 300, behavior: 'smooth' });
}

// ── Обработчики ──────────────────────────────────────────────────
document.querySelectorAll('input[data-filter]').forEach(cb => {
    cb.addEventListener('change', () => { currentPage = 1; updateGrid(1); });
});

['year-from', 'year-to'].forEach(function(id) {
    document.getElementById(id)?.addEventListener('input', () => {
        clearTimeout(yearTimer);
        yearTimer = setTimeout(() => { currentPage = 1; updateGrid(1); }, 500);
    });
});

document.getElementById('sort-select')?.addEventListener('change', () => { currentPage = 1; updateGrid(1); });

document.getElementById('reset-btn')?.addEventListener('click', () => {
    document.querySelectorAll('input[data-filter]').forEach(cb => cb.checked = false);
    ['search-input','year-from','year-to'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
    });
    const ss = document.getElementById('sort-select');
    if (ss) ss.value = 'name-asc';
    if (suggestionsBox) suggestionsBox.style.display = 'none';
    currentPage = 1;
    updateGrid(1);
});

function escapeHtml(str) {
    if (!str) return '';
    return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#39;');
}

function showNotification(msg, type) {
    const n = document.createElement('div');
    n.className = 'notification notification-' + (type || 'success');
    n.textContent = msg;
    document.body.appendChild(n);
    setTimeout(() => n.remove(), 3000);
}
