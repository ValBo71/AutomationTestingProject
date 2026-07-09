# TS-Playwright-Project

Това е automation проект, изграден с Playwright и TypeScript, базиран на Page Object Model (POM) патърн.

## Структура на проекта

Проектът следва следните най-добри практики:
- **POM**: За всяка страница има отделен Page клас (в `pages/`), съдържащ само методи (actions).
- **Селектори**: Изнесени са в отделни файлове (в `selectors/`).
- **Тестови данни**: Изнесени са във външни файлове (в `data/`).
- **Твърдения (Assertions)**: Playwright `expect` се използва само в самите тестове.

## Инсталация

След като създадете проекта, инсталирайте нужните зависимости:

```bash
npm install
npx playwright install
```

## Стартиране на тестове

За да стартирате всички тестове в **headless** режим:

```bash
npx playwright test
```

За да стартирате всички тестове в **headed** режим:

```bash
npx playwright test --headed
```

Или можете да използвате NPM скриптовете:

```bash
npm test
npm run test:headed
```
