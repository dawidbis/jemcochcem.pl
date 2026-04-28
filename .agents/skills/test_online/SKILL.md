---
name: testonline
description: Wykonuje testy funkcjonalności działającej aplikacji webowej na serwerze online
---

# Test Online - Live Application Testing Skill

Skill do testowania działającej aplikacji webowej na serwerze online (staging/production).

## Kiedy używać

- Weryfikacja deploymentu na staging/production
- Smoke tests po wdrożeniu
- End-to-End testing na żywym środowisku
- Weryfikacja integracji z zewnętrznymi API
- Performance testing w warunkach produkcyjnych
- Monitoring i health checks

## Konfiguracja

### Zmienne środowiskowe
```bash
# .env.test
TEST_SERVER_URL=https://staging.jemcochcem.pl
# lub
TEST_SERVER_URL=https://jemcochcem.pl

API_BASE_URL=${TEST_SERVER_URL}/api
TEST_USER_EMAIL=test@example.com
TEST_USER_PASSWORD=SecurePassword123!
```

### Nazwa serwera
Przed rozpoczęciem testów, określ nazwę serwera:
- **Staging**: `https://staging.jemcochcem.pl`
- **Production**: `https://jemcochcem.pl`
- **Local**: `http://localhost:5000` (dla testów lokalnych)

## Typy testów online

### 1. Smoke Tests
Szybkie testy sprawdzające czy podstawowe funkcjonalności działają.

```typescript
/**
 * [CO] Smoke tests dla aplikacji online
 * [DLACZEGO] Szybka weryfikacja po deploymencie
 * [JAK] Testuje kluczowe endpointy i funkcjonalności
 */

import { test, expect } from '@playwright/test';

const SERVER_URL = process.env.TEST_SERVER_URL || 'https://staging.jemcochcem.pl';

test.describe('Smoke Tests - Online Server', () => {
  test('Server is responding', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/health`);
    expect(response.status()).toBe(200);
  });

  test('Homepage loads successfully', async ({ page }) => {
    await page.goto(SERVER_URL);
    await expect(page).toHaveTitle(/FitApp|jemcochcem/i);
  });

  test('API is accessible', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/api/health`);
    expect(response.status()).toBe(200);
    
    const data = await response.json();
    expect(data.status).toBe('healthy');
  });

  test('Login page is accessible', async ({ page }) => {
    await page.goto(`${SERVER_URL}/login`);
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
  });
});
```

### 2. End-to-End Tests

```typescript
/**
 * E2E test - Pełny flow użytkownika
 */

import { test, expect } from '@playwright/test';

const SERVER_URL = process.env.TEST_SERVER_URL || 'https://staging.jemcochcem.pl';
const TEST_EMAIL = process.env.TEST_USER_EMAIL || 'test@example.com';
const TEST_PASSWORD = process.env.TEST_USER_PASSWORD || 'TestPassword123!';

test.describe('E2E - User Journey', () => {
  test('Complete user flow: Login -> Add Meal -> View Diary -> Logout', async ({ page }) => {
    // 1. Login
    await page.goto(`${SERVER_URL}/login`);
    await page.fill('input[type="email"]', TEST_EMAIL);
    await page.fill('input[type="password"]', TEST_PASSWORD);
    await page.click('button[type="submit"]');
    
    // Sprawdź czy zalogowano
    await expect(page).toHaveURL(/\/dashboard|\/diary/);
    
    // 2. Przejdź do dziennika
    await page.click('a[href*="/diary"]');
    await expect(page).toHaveURL(/\/diary/);
    
    // 3. Dodaj posiłek
    await page.click('button:has-text("Add Meal")');
    await page.fill('input[name="foodName"]', 'Test Apple');
    await page.fill('input[name="calories"]', '95');
    await page.click('button[type="submit"]');
    
    // Sprawdź czy posiłek został dodany
    await expect(page.locator('text=Test Apple')).toBeVisible();
    await expect(page.locator('text=95 kcal')).toBeVisible();
    
    // 4. Usuń testowy posiłek (cleanup)
    await page.click('button:has-text("Delete"):near(:text("Test Apple"))');
    await expect(page.locator('text=Test Apple')).not.toBeVisible();
    
    // 5. Logout
    await page.click('button:has-text("Logout")');
    await expect(page).toHaveURL(/\/login/);
  });
});
```

### 3. API Integration Tests

```typescript
/**
 * Testy integracyjne API na żywym serwerze
 */

import { test, expect } from '@playwright/test';

const API_URL = `${process.env.TEST_SERVER_URL}/api` || 'https://staging.jemcochcem.pl/api';
let authToken: string;

test.describe('API Integration Tests - Online', () => {
  test.beforeAll(async ({ request }) => {
    // Zaloguj się i pobierz token
    const response = await request.post(`${API_URL}/auth/login`, {
      data: {
        email: process.env.TEST_USER_EMAIL,
        password: process.env.TEST_USER_PASSWORD,
      },
    });
    
    expect(response.status()).toBe(200);
    const data = await response.json();
    authToken = data.token;
  });

  test('GET /api/diary - Returns user diary', async ({ request }) => {
    const today = new Date().toISOString().split('T')[0];
    
    const response = await request.get(`${API_URL}/diary?date=${today}`, {
      headers: {
        'Authorization': `Bearer ${authToken}`,
      },
    });
    
    expect(response.status()).toBe(200);
    const data = await response.json();
    expect(data).toHaveProperty('mealLogs');
    expect(Array.isArray(data.mealLogs)).toBe(true);
  });

  test('POST /api/diary - Adds meal item', async ({ request }) => {
    const response = await request.post(`${API_URL}/diary`, {
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json',
      },
      data: {
        date: new Date().toISOString(),
        foodName: 'Test Banana',
        calories: 105,
        protein: 1.3,
        carbs: 27,
        fat: 0.4,
      },
    });
    
    expect(response.status()).toBe(201);
    const mealLogId = await response.json();
    expect(mealLogId).toBeTruthy();
    
    // Cleanup - usuń testowy posiłek
    await request.delete(`${API_URL}/diary/${mealLogId}`, {
      headers: {
        'Authorization': `Bearer ${authToken}`,
      },
    });
  });

  test('External API - Open Food Facts integration', async ({ request }) => {
    const response = await request.get(`${API_URL}/food/search?query=apple`, {
      headers: {
        'Authorization': `Bearer ${authToken}`,
      },
    });
    
    expect(response.status()).toBe(200);
    const data = await response.json();
    expect(data.products).toBeDefined();
    expect(data.products.length).toBeGreaterThan(0);
  });
});
```

### 4. Performance Tests

```typescript
/**
 * Testy wydajnościowe na żywym serwerze
 */

import { test, expect } from '@playwright/test';

const SERVER_URL = process.env.TEST_SERVER_URL || 'https://staging.jemcochcem.pl';

test.describe('Performance Tests - Online', () => {
  test('Homepage loads within acceptable time', async ({ page }) => {
    const startTime = Date.now();
    
    await page.goto(SERVER_URL);
    await page.waitForLoadState('networkidle');
    
    const loadTime = Date.now() - startTime;
    
    console.log(`Homepage load time: ${loadTime}ms`);
    expect(loadTime).toBeLessThan(3000); // Max 3 sekundy
  });

  test('API response time is acceptable', async ({ request }) => {
    const startTime = Date.now();
    
    const response = await request.get(`${SERVER_URL}/api/health`);
    
    const responseTime = Date.now() - startTime;
    
    console.log(`API response time: ${responseTime}ms`);
    expect(response.status()).toBe(200);
    expect(responseTime).toBeLessThan(500); // Max 500ms
  });

  test('Multiple concurrent requests', async ({ request }) => {
    const requests = Array(10).fill(null).map(() => 
      request.get(`${SERVER_URL}/api/health`)
    );
    
    const startTime = Date.now();
    const responses = await Promise.all(requests);
    const totalTime = Date.now() - startTime;
    
    console.log(`10 concurrent requests completed in: ${totalTime}ms`);
    
    responses.forEach(response => {
      expect(response.status()).toBe(200);
    });
    
    expect(totalTime).toBeLessThan(2000); // Max 2 sekundy dla 10 requestów
  });
});
```

### 5. Security Tests

```typescript
/**
 * Testy bezpieczeństwa na żywym serwerze
 */

import { test, expect } from '@playwright/test';

const API_URL = `${process.env.TEST_SERVER_URL}/api` || 'https://staging.jemcochcem.pl/api';

test.describe('Security Tests - Online', () => {
  test('Unauthorized access is blocked', async ({ request }) => {
    const response = await request.get(`${API_URL}/diary`);
    
    expect(response.status()).toBe(401); // Unauthorized
  });

  test('Invalid token is rejected', async ({ request }) => {
    const response = await request.get(`${API_URL}/diary`, {
      headers: {
        'Authorization': 'Bearer invalid-token-12345',
      },
    });
    
    expect(response.status()).toBe(401);
  });

  test('SQL Injection attempt is blocked', async ({ request }) => {
    const response = await request.get(`${API_URL}/food/search?query=' OR '1'='1`);
    
    // Powinno zwrócić 400 (Bad Request) lub 200 z pustym wynikiem
    expect([200, 400]).toContain(response.status());
    
    if (response.status() === 200) {
      const data = await response.json();
      // Nie powinno zwrócić wszystkich produktów
      expect(data.products?.length || 0).toBeLessThan(100);
    }
  });

  test('XSS attempt is sanitized', async ({ page }) => {
    await page.goto(`${process.env.TEST_SERVER_URL}/login`);
    
    // Próba XSS w polu email
    await page.fill('input[type="email"]', '<script>alert("XSS")</script>');
    await page.fill('input[type="password"]', 'password');
    await page.click('button[type="submit"]');
    
    // Sprawdź czy nie wykonał się script
    const alerts = [];
    page.on('dialog', dialog => {
      alerts.push(dialog.message());
      dialog.dismiss();
    });
    
    await page.waitForTimeout(1000);
    expect(alerts.length).toBe(0);
  });

  test('HTTPS is enforced', async ({ request }) => {
    if (process.env.TEST_SERVER_URL?.startsWith('https://')) {
      const httpUrl = process.env.TEST_SERVER_URL.replace('https://', 'http://');
      
      const response = await request.get(httpUrl, {
        maxRedirects: 0,
      });
      
      // Powinno przekierować na HTTPS
      expect([301, 302, 307, 308]).toContain(response.status());
    }
  });
});
```

### 6. Health Checks & Monitoring

```typescript
/**
 * Health checks i monitoring
 */

import { test, expect } from '@playwright/test';

const SERVER_URL = process.env.TEST_SERVER_URL || 'https://staging.jemcochcem.pl';

test.describe('Health Checks - Online', () => {
  test('Application health endpoint', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/health`);
    
    expect(response.status()).toBe(200);
    
    const health = await response.json();
    expect(health.status).toBe('healthy');
    expect(health.timestamp).toBeDefined();
  });

  test('Database connectivity', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/health/database`);
    
    expect(response.status()).toBe(200);
    
    const dbHealth = await response.json();
    expect(dbHealth.database).toBe('connected');
  });

  test('External API connectivity - Open Food Facts', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/health/external-apis`);
    
    expect(response.status()).toBe(200);
    
    const apiHealth = await response.json();
    expect(apiHealth.openFoodFacts).toBe('available');
  });

  test('Redis cache connectivity', async ({ request }) => {
    const response = await request.get(`${SERVER_URL}/health/cache`);
    
    expect(response.status()).toBe(200);
    
    const cacheHealth = await response.json();
    expect(cacheHealth.redis).toBe('connected');
  });
});
```

## Setup Playwright dla testów online

### Instalacja
```bash
# Frontend directory
npm install -D @playwright/test
npx playwright install
```

### Konfiguracja playwright.config.ts
```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests/e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  
  use: {
    baseURL: process.env.TEST_SERVER_URL || 'https://staging.jemcochcem.pl',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
  ],
});
```

## Uruchamianie testów online

### Podstawowe komendy
```bash
# Wszystkie testy na staging
TEST_SERVER_URL=https://staging.jemcochcem.pl npx playwright test

# Smoke tests
npx playwright test smoke

# E2E tests
npx playwright test e2e

# Z UI mode
npx playwright test --ui

# Konkretny plik
npx playwright test diary.spec.ts

# Z raportem
npx playwright test --reporter=html
npx playwright show-report
```

### CI/CD Integration
```yaml
# .github/workflows/test-online.yml
name: Online Tests

on:
  deployment_status:

jobs:
  test:
    if: github.event.deployment_status.state == 'success'
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        run: npm ci
      
      - name: Install Playwright
        run: npx playwright install --with-deps
      
      - name: Run smoke tests
        env:
          TEST_SERVER_URL: ${{ github.event.deployment_status.target_url }}
          TEST_USER_EMAIL: ${{ secrets.TEST_USER_EMAIL }}
          TEST_USER_PASSWORD: ${{ secrets.TEST_USER_PASSWORD }}
        run: npx playwright test smoke
      
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: playwright-report/
```

## Checklist testów online

### Przed testami
- [ ] Określ nazwę serwera (staging/production)
- [ ] Skonfiguruj zmienne środowiskowe
- [ ] Przygotuj test user account
- [ ] Sprawdź czy serwer jest dostępny

### Smoke Tests (zawsze)
- [ ] Server responds (health check)
- [ ] Homepage loads
- [ ] API is accessible
- [ ] Login page works
- [ ] Critical endpoints respond

### E2E Tests (staging)
- [ ] User registration flow
- [ ] Login/Logout flow
- [ ] Add/Edit/Delete meal
- [ ] View diary
- [ ] Search food
- [ ] Profile management

### Performance Tests
- [ ] Page load time <3s
- [ ] API response time <500ms
- [ ] Concurrent requests handling
- [ ] Database query performance

### Security Tests
- [ ] Authentication required
- [ ] Authorization checks
- [ ] SQL injection prevention
- [ ] XSS prevention
- [ ] HTTPS enforcement

### Monitoring
- [ ] Application health
- [ ] Database connectivity
- [ ] External APIs availability
- [ ] Cache connectivity

## Best Practices

### 1. Używaj test user accounts
```typescript
// Nie testuj na prawdziwych użytkownikach!
const TEST_USERS = {
  staging: {
    email: 'test.staging@example.com',
    password: 'StagingPassword123!',
  },
  production: {
    email: 'test.production@example.com',
    password: 'ProductionPassword123!',
  },
};
```

### 2. Cleanup po testach
```typescript
test.afterEach(async ({ request }) => {
  // Usuń testowe dane
  await request.delete(`${API_URL}/test-data/cleanup`, {
    headers: { 'Authorization': `Bearer ${authToken}` },
  });
});
```

### 3. Retry dla flaky tests
```typescript
test.describe('Flaky tests', () => {
  test.use({ retries: 3 }); // Retry 3 razy
  
  test('Sometimes fails due to network', async ({ page }) => {
    // Test code
  });
});
```

### 4. Timeouts dla slow operations
```typescript
test('Slow operation', async ({ page }) => {
  test.setTimeout(60000); // 60 sekund
  
  await page.goto(SERVER_URL);
  // Slow operation
});
```

## Przykład użycia

```
User: "Przetestuj aplikację na serwerze staging.jemcochcem.pl"

Cline (używając skill test_online):
1. Konfiguruje TEST_SERVER_URL=https://staging.jemcochcem.pl
2. Uruchamia smoke tests
3. Uruchamia E2E tests
4. Sprawdza performance
5. Weryfikuje security
6. Sprawdza health checks
7. Generuje raport z wynikami
```

## Monitoring ciągły

### Scheduled Tests (Cron)
```yaml
# .github/workflows/scheduled-tests.yml
name: Scheduled Online Tests

on:
  schedule:
    - cron: '0 */6 * * *' # Co 6 godzin

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run smoke tests
        run: npx playwright test smoke
      - name: Notify on failure
        if: failure()
        uses: 8398a7/action-slack@v3
        with:
          status: ${{ job.status }}
          text: 'Online tests failed!'
```
