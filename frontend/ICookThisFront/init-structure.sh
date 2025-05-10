#!/usr/bin/env bash
set -e

# root folder
ROOT=src

# 1. katalogi
dirs=(
  "$ROOT/app"
  "$ROOT/processes"
  "$ROOT/pages"
  "$ROOT/widgets"
  "$ROOT/features/recipes"
  "$ROOT/features/recipeDetail"
  "$ROOT/features/reviews"
  "$ROOT/entities/recipe"
  "$ROOT/entities/ingredient"
  "$ROOT/entities/unit"
  "$ROOT/entities/review"
  "$ROOT/shared/ui"
  "$ROOT/shared/lib"
  "$ROOT/assets/images"
  "$ROOT/assets/styles"
  "$ROOT/router"
  "$ROOT/store"
  "$ROOT/tests/unit"
  "$ROOT/tests/e2e"
)

for d in "${dirs[@]}"; do
  mkdir -p "$d"
done

# 2. puste pliki
files=(
  # app
  "$ROOT/app/App.vue"
  "$ROOT/app/main.ts"
  "$ROOT/app/app.scss"

  # pages
  "$ROOT/pages/HomePage.vue"
  "$ROOT/pages/RecipeListPage.vue"
  "$ROOT/pages/RecipeDetailPage.vue"

  # widgets
  "$ROOT/widgets/RecipeCardWidget.vue"
  "$ROOT/widgets/PaginationWidget.vue"
  "$ROOT/widgets/ReviewListWidget.vue"

  # features recipes
  "$ROOT/features/recipes/useFetchRecipes.ts"
  "$ROOT/features/recipes/RecipeFilters.vue"

  # features recipeDetail
  "$ROOT/features/recipeDetail/useFetchRecipe.ts"
  "$ROOT/features/recipeDetail/ScaleControl.vue"

  # features reviews
  "$ROOT/features/reviews/ReviewForm.vue"
  "$ROOT/features/reviews/useSubmitReview.ts"

  # entities recipe
  "$ROOT/entities/recipe/recipeModel.ts"
  "$ROOT/entities/recipe/recipeService.ts"

  # entities ingredient
  "$ROOT/entities/ingredient/ingredientModel.ts"
  "$ROOT/entities/ingredient/ingredientService.ts"

  # entities unit
  "$ROOT/entities/unit/unitModel.ts"
  "$ROOT/entities/unit/unitService.ts"

  # entities review
  "$ROOT/entities/review/reviewModel.ts"
  "$ROOT/entities/review/reviewService.ts"

  # shared ui
  "$ROOT/shared/ui/BaseButton.vue"
  "$ROOT/shared/ui/BaseInput.vue"
  "$ROOT/shared/ui/Loader.vue"

  # shared lib
  "$ROOT/shared/lib/api.ts"
  "$ROOT/shared/lib/helpers.ts"

  # router & store
  "$ROOT/router/index.ts"
  "$ROOT/store/index.ts"

  # tests
  "$ROOT/tests/unit/recipeService.spec.ts"
  "$ROOT/tests/unit/reviewForm.spec.ts"
  "$ROOT/tests/e2e/recipes-flow.spec.ts"
)

for f in "${files[@]}"; do
  # only create if not exists
  if [ ! -e "$f" ]; then
    touch "$f"
  fi
done

echo "Struktura Feature-Sliced została wygenerowana w katalogu $ROOT/"