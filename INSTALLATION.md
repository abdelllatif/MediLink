# 🚀 Guide d'Installation - MediLink

## Prérequis

### Pour le Backend
- **.NET 8.0 SDK** ou supérieur: https://dotnet.microsoft.com/download
- **SQL Server 2019+** ou **PostgreSQL 13+**
- **Visual Studio 2022** ou **VS Code**
- **Git**

### Pour le Frontend
- **Node.js 18+**: https://nodejs.org/
- **npm 9+** ou **yarn 3+**
- **VS Code** (recommandé)

### Optionnel
- **Docker & Docker Compose** (pour containerisation)
- **Postman** (pour tester l'API)
- **Azure CLI** (pour déploiement)

---

## Installation Locale

### 1. Cloner le Repository

```bash
git clone https://github.com/your-org/MediLink.git
cd MediLink
```

### 2. Configuration Backend

#### 2.1 Restaurer les Dépendances

```bash
dotnet restore
```

#### 2.2 Configurer la Base de Données

##### Avec SQL Server (LocalDB)

```bash
# Visual Studio 2022
# Ouvrir SQL Server Object Explorer
# Créer une base de données nommée "MediLink"

# Ou via terminal:
sqlcmd -S "(localdb)\mssqllocaldb" -i setup.sql
```

##### Avec Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyP@ssw0rd123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2019-latest
```

#### 2.3 Appliquer les Migrations

```bash
cd src/MediLink.Infrastructure

# Ajouter les migrations (si non existantes)
dotnet ef migrations add InitialCreate

# Appliquer les migrations
dotnet ef database update

cd ../..
```

#### 2.4 Variables d'Environnement

Créer un fichier `.env` depuis `.env.example`:

```bash
cp .env.example .env
```

Éditer les valeurs sensibles dans `.env`:

```
# Database
DB_CONNECTION_STRING=Server=(localdb)\mssqllocaldb;Database=MediLink;Trusted_Connection=true;

# JWT
JWT_SECRET_KEY=your-super-secret-key-min-32-chars-change-this-in-production
JWT_ISSUER=MediLink
JWT_AUDIENCE=MediLink

# Stripe
STRIPE_SECRET_KEY=sk_test_your_key_here
STRIPE_PUBLISHABLE_KEY=pk_test_your_key_here
STRIPE_WEBHOOK_SECRET=whsec_your_webhook_secret_here

# Email (SendGrid)
SENDGRID_API_KEY=SG.your-api-key-here

# Frontend CORS
CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000
```

#### 2.5 Démarrer le Backend

```bash
cd src/MediLink.API

dotnet run

# L'API sera disponible sur: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger/index.html
```

---

### 3. Configuration Frontend

#### 3.1 Installer les Dépendances

```bash
cd frontend

npm install
# ou
yarn install
```

#### 3.2 Variables d'Environnement

Créer un fichier `.env.local`:

```bash
cat > frontend/.env.local << EOF
VITE_API_BASE_URL=http://localhost:5000
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_your_key_here
EOF
```

#### 3.3 Démarrer le Serveur de Développement

```bash
npm run dev

# Frontend sera disponible sur: http://localhost:5173
```

---

## Installation avec Docker

### Prérequis
- Docker Desktop installé: https://www.docker.com/products/docker-desktop

### Démarrage

```bash
# Depuis la racine du projet
docker-compose up -d

# Attendre que les services démarrent (30-45 secondes)
docker-compose logs -f

# Pour arrêter:
docker-compose down
```

Services disponibles:
- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger/index.html
- **SQL Server**: localhost:1433
  - User: sa
  - Password: MyP@ssw0rd123!
- **Redis**: localhost:6379

---

## Tester l'Application

### Swagger UI (API)

Accéder à: http://localhost:5000/swagger/index.html

### Postman

1. Importer la collection Postman depuis: `docs/postman-collection.json`
2. Configurer l'environnement avec:
   - `baseUrl`: http://localhost:5000
   - `token`: (obtenu après login)

### Frontend

Accéder à: http://localhost:5173

Comptes de test:
```
Admin:
Email: admin@medilink.com
Password: Admin@123

Doctor:
Email: doctor@medilink.com
Password: Doctor@123

Patient:
Email: patient@medilink.com
Password: Patient@123
```

---

## Tester l'API Manuellement

### 1. S'authentifier

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@medilink.com",
    "password": "Admin@123"
  }'

# Réponse:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "...",
  "expiresIn": 3600
}
```

### 2. Récupérer les Patients

```bash
curl -X GET http://localhost:5000/api/patients \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Créer un Patient

```bash
curl -X POST http://localhost:5000/api/patients \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phone": "+212612345678",
    "dateOfBirth": "1990-01-15"
  }'
```

---

## Commandes Utiles

### Backend

```bash
# Build
dotnet build

# Run tests
dotnet test

# Run specific project
dotnet run -p src/MediLink.API

# Add migration
dotnet ef migrations add MigrationName -p src/MediLink.Infrastructure

# Update database
dotnet ef database update -p src/MediLink.Infrastructure

# Remove last migration
dotnet ef migrations remove -p src/MediLink.Infrastructure

# Generate migration script
dotnet ef migrations script -p src/MediLink.Infrastructure -o migration.sql
```

### Frontend

```bash
# Dev server
npm run dev

# Build production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint

# Run tests
npm run test

# Type checking
npm run type-check
```

---

## Résolution de Problèmes

### Port déjà utilisé

```bash
# Trouver le processus utilisant le port 5000
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Tuer le processus
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

### Erreur de connexion à la BD

```bash
# Vérifier que SQL Server est démarré
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT 1"

# Réinitialiser les migrations
dotnet ef database drop -p src/MediLink.Infrastructure
dotnet ef database update -p src/MediLink.Infrastructure
```

### Erreur CORS

Ajouter le domaine frontend dans `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  }
}
```

### Erreur JWT

Vérifier que la clé secrète JWT:
- Fait au moins 32 caractères
- Est identique dans `.env` et `appsettings.json`
- Pas exposée en public (ne pas committer `.env`)

---

## Étapes Suivantes

1. **Consulter la documentation**: [README.md](README.md)
2. **Lire le cahier des charges**: [docs/CAHIER_DES_CHARGES.md](docs/CAHIER_DES_CHARGES.md)
3. **Comprendre l'architecture**: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
4. **Contribuer au projet**: [CONTRIBUTING.md](CONTRIBUTING.md)

---

## Support

Pour toute question ou problème:
- 📧 Email: support@medilink.com
- 🐛 Issues: https://github.com/your-org/MediLink/issues
- 📚 Documentation: [docs/](docs/)

---

**Bienvenue dans MediLink!** 🏥✨
