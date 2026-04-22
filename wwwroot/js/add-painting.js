

const form       = document.getElementById('add-painting-form');
const submitBtn  = document.getElementById('submit-btn');
let   dupTimer   = null;

// ── Правила валидации на клиенте ─────────────────────────────────
const rules = {
    'field-title': {
        required: 'Введите название картины',
        minLength: [2, 'Минимум 2 символа'],
        pattern:  [/^[А-ЯA-Z«(]/, 'Первый символ должен быть заглавной буквой или «'],
    },
    'field-author': {
        required: 'Введите имя автора',
        minLength: [2, 'Минимум 2 символа'],
        pattern:  [/^[А-ЯA-Z]/, 'Имя должно начинаться с заглавной буквы'],
    },
    'field-year': {
        required: 'Введите год создания',
        range:    [1000, new Date().getFullYear(),
                   'Год должен быть от 1000 до ' + new Date().getFullYear()],
    },
    'field-style': {
        required: 'Выберите стиль из списка',
    },
    'field-country': {
        required: 'Выберите страну из списка',
    },
    'field-size': {
        pattern: [/^(\d+(\.\d+)?\s?[×xх]\s?\d+(\.\d+)?\s?см)?$/,
                  'Формат: 73.7 × 92.1 см (поле необязательное)'],
    },
};

// ── Валидация одного поля ────────────────────────────────────────
function validateField(el) {
    const id  = el.id;
    const val = el.value.trim();
    const r   = rules[id];
    if (!r) return true;

    let error = '';

    if (r.required && val === '')          { error = r.required; }
    else if (r.minLength && val.length > 0 && val.length < r.minLength[0])
                                           { error = r.minLength[1]; }
    else if (r.pattern && val.length > 0 && !r.pattern[0].test(val))
                                           { error = r.pattern[1]; }
    else if (r.range && val !== '') {
        const n = parseInt(val, 10);
        if (isNaN(n) || n < r.range[0] || n > r.range[1])
                                           { error = r.range[2]; }
    }

    showFieldError(el, error);
    return error === '';
}

function showFieldError(el, msg) {
    el.classList.toggle('input-error', msg !== '');
    el.classList.toggle('input-ok',    msg === '' && el.value.trim() !== '');

    // Ищем span с классом field-error рядом с полем
    const wrap = el.closest('.form-group');
    if (!wrap) return;
    const errSpan = wrap.querySelector('.field-error');
    if (errSpan) errSpan.textContent = msg;
}

// ── Проверка дубликата через AJAX ────────────────────────────────
function checkDuplicate() {
    const title  = document.getElementById('field-title')?.value.trim()  || '';
    const author = document.getElementById('field-author')?.value.trim() || '';
    const warn   = document.getElementById('title-duplicate');
    if (!warn || title.length < 2) { if (warn) warn.style.display = 'none'; return; }

    clearTimeout(dupTimer);
    dupTimer = setTimeout(() => {
        fetch('/AddPainting/CheckTitle?title=' + encodeURIComponent(title) +
              '&author=' + encodeURIComponent(author))
            .then(r => r.json())
            .then(data => {
                warn.style.display = data.exists ? 'block' : 'none';
            });
    }, 400);
}

// ── Предпросмотр загруженного файла ─────────────────────────────────────
document.getElementById('field-image')?.addEventListener('change', function (e) {
    const file = e.target.files[0];
    const container = document.getElementById('image-preview-container');
    const preview = document.getElementById('image-preview');
    const hiddenUrl = document.getElementById('ImageUrl');

    if (!container || !preview) return;

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            container.style.display = 'block';
        };
        reader.readAsDataURL(file);

        // Очищаем скрытое поле URL, так как используем файл
        if (hiddenUrl) hiddenUrl.value = '';
    } else {
        container.style.display = 'none';
    }
});

// ── Навешиваем live-валидацию на все поля ────────────────────────
Object.keys(rules).forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;

    el.addEventListener('blur',  () => validateField(el));
    el.addEventListener('input', () => {
        if (el.classList.contains('input-error')) validateField(el);
        if (id === 'field-title' || id === 'field-author') checkDuplicate();
    });
    el.addEventListener('change', () => validateField(el));
});

// ── Валидация всей формы перед отправкой ────────────────────────
form?.addEventListener('submit', function (e) {
    let valid = true;

    Object.keys(rules).forEach(id => {
        const el = document.getElementById(id);
        if (el && !validateField(el)) valid = false;
    });

    // Блокируем если дубликат
    const warn = document.getElementById('title-duplicate');
    if (warn && warn.style.display !== 'none') valid = false;

    if (!valid) {
        e.preventDefault();
        // Прокрутка к первой ошибке
        const firstError = form.querySelector('.input-error');
        if (firstError) firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });

        // Кратко трясём кнопку
        submitBtn?.classList.add('btn-shake');
        setTimeout(() => submitBtn?.classList.remove('btn-shake'), 500);
        return;
    }

    // Блокируем повторную отправку
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Сохранение...';
    }
});
