<p align="center">
  <img src="https://raw.githubusercontent.com/Kahalix/ICookThis/main/docs/logo.png" alt="ICookThis Logo" width="200"/>
</p>

# ICookThis ⋅ ASP .NET Core REST API with Vue.js  
> A modular cooking platform: users, recipes, reviews & votes.

---

## 📖 Project Overview  
ICookThis is a cleanly layered ASP .NET Core Web API, featuring:
- **Authentication & Authorization**: email confirmation, password reset tokens.
- **User Management**: roles (Admin/Moderator/User), status (Pending/Approved/Suspended/Deleted), profile images.
- **Recipes**: create, read, update, search, filter; ingredients, units, instruction steps.
- **Reviews & Votes**: star ratings, difficulty, preparation time, “Recommend” flag, moderation workflow, vote-based “helpfulness”.
- **Business Logic**: automatic recalculation of recipe statistics (avg rating, difficulty, recommend%), and author trust factors.
- **Email Notifications**: styled HTML templates for confirmations, resets, admin actions, recipe/review events.
- **Background Service**: daily cleanup of stale pending accounts.

---

## 📦 Modules  
- **Auth**  
  Entities: `UserToken`, `TokenType`  
  Features: registration, login, JWT issuance, token-based email workflows.

- **Users**  
  Entities: `User`, `UserRole`, `UserStatus`  
  Services: CRUD, profile image management, status & role changes with notifications.

- **Ingredients & Units**  
  Entities: `Ingredient`, `Unit`, `UnitType`  
  Simple CRUD for lookup tables.

- **Recipes**  
  Entities: `Recipe`, `RecipeStatus`, `DishType`, `AddedBy`, plus `InstructionStep`, `RecipeIngredient`, `StepIngredient`.  
  Features: paged listing, search, filtering, scaleable instructions via placeholder replacement.

- **Reviews**  
  Entities: `Review`, `ReviewStatus`, `ReviewVote`  
  Features: paged reviews per recipe/user, moderation, vote-driven trust.

- **Shared**  
  DTOs: `PagedResult<T>`  
  Helpers: `EmailTemplateBuilder` (central CSS-styled layout).

- **Utils**  
  - `PlaceholderReplacer`: injects ingredient quantities into step templates, e.g. turning `"Boil {Water}."` into `"450 ml water"` based on recipe and step fractions.  
  - `EmailBuilder`: high-level constructors for all notification emails, wrapping content in the shared HTML template.

---

## 🖼 Static Assets  
All user-uploaded images and generated pictures are served from **wwwroot** subfolders:
wwwroot/
├─ images/
│ ├─ recipes/
│ ├─ instructionsteps/
│ └─ users/
│ ├─ profiles/
│ └─ banners/

---

## 🔧 PlaceholderReplacer  
```csharp
// Finds {IngredientName} in step.TemplateText,
// looks up corresponding RecipeIngredient and StepIngredient,
// and computes: formattedQty = baseQty * fraction * scale.
string replaced = PlaceholderReplacer.Replace(
    template: "Add {Sugar}.", 
    recipeIngredients, stepIngredients, scale: 1.5m
);
```
// → "150 g sugar"

---

📨 Email Notification Flow
All emails use EmailTemplateBuilder.Wrap(title, innerHtml) for consistent branding:

Account confirmation

Password reset

Admin-created user

Status & role changes

Recipe created / status changed

Review created / status changed

Each builder method returns (Subject, BodyHtml) and is sent via MailService (MailKit + config).

---

📊 Database Design
Entity-Relationship diagram:

![image](https://github.com/user-attachments/assets/cc90e1c4-0a82-481a-9a6e-0aee08d6b77a)

---

Project documents including ER diagrams, sequence diagrams and full API spec will be/are in the docs/ folder.
