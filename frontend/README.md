# 🎨 MediLink Frontend

Frontend React pour la plateforme MediLink - Système de gestion du parcours médical.

## 🛠️ Tech Stack

- **React 18** - UI Framework
- **TypeScript 5** - Type safety
- **Vite** - Build tool (ultra-rapide)
- **Tailwind CSS** - Styling
- **React Query** - Data fetching & caching
- **React Hook Form** - Form management
- **Zod** - Schema validation
- **Axios** - HTTP client
- **Stripe.js** - Payment integration
- **FullCalendar** - Appointment calendar
- **SignalR** - Real-time updates

## 📁 Structure

```
src/
├── pages/           # Page components
├── components/      # Reusable components
├── services/        # API services
├── hooks/           # Custom React hooks
├── context/         # React context
├── utils/           # Helper functions
├── types/           # TypeScript interfaces
├── styles/          # Global styles
├── App.tsx          # Main app component
└── main.tsx         # Entry point
```

## 🚀 Quick Start

```bash
# Install dependencies
npm install

# Start dev server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run tests
npm run test

# Lint code
npm run lint
```

## 📋 Features

### Authentication
- Login/Register
- JWT token management
- Role-based access control

### Patient Module
- View profile
- Book appointments
- View medical documents
- Payment history

### Doctor Module
- Manage schedule
- View appointments
- Create consultations
- Block days

### Admin Module
- User management
- System configuration
- Reports & analytics

## 🔗 API Integration

Base URL: `http://localhost:5000`

Key endpoints:
```
POST   /api/auth/login
POST   /api/auth/register
GET    /api/patients
POST   /api/patients
GET    /api/appointments
POST   /api/appointments
GET    /api/timeslots
POST   /api/payments/create-checkout-session
```

See [docs/API_DOCUMENTATION.md](../docs/API_DOCUMENTATION.md) for complete API reference.

## 🧪 Testing

```bash
# Run all tests
npm run test

# Watch mode
npm run test -- --watch

# Coverage
npm run test -- --coverage
```

## 📦 Building

```bash
# Production build
npm run build

# Output in dist/
# Ready to deploy to static hosting (Vercel, Netlify, Azure Static Web Apps)
```

## 🔒 Environment Variables

Create `.env.local`:

```
VITE_API_BASE_URL=http://localhost:5000
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_xxx
```

## 📚 Documentation

- [README (main)](../README.md)
- [Installation Guide](../INSTALLATION.md)
- [Architecture](../docs/ARCHITECTURE.md)
- [Contributing](../CONTRIBUTING.md)

## 🤝 Contributing

1. Create feature branch: `git checkout -b feature/amazing-feature`
2. Commit changes: `git commit -m 'feat: add amazing feature'`
3. Push to branch: `git push origin feature/amazing-feature`
4. Open a Pull Request

## 📝 License

MIT - see LICENSE file

---

**MediLink Frontend** © 2024
