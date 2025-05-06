# ICookThis
Food recipes and reviews web app ASP .Net Core Vue.js





Endpoints for testing (import to postman):

# 1. Ingredients (JSON)

# Get paged
curl -X GET "http://localhost:5284/api/ingredients?page=1&pageSize=10&search=flour" \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/ingredients/2 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/ingredients \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"name":"Sugar"}'

# Update
curl -X PUT http://localhost:5284/api/ingredients/2 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"name":"Brown Sugar"}'

# Delete
curl -X DELETE http://localhost:5284/api/ingredients/2


# 2. Units (JSON)

# Get all
curl -X GET http://localhost:5284/api/units \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/units/2 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/units \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"symbol":"g","type":"Mass"}'

# Update
curl -X PUT http://localhost:5284/api/units/2 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"symbol":"kg","type":"Mass"}'

# Delete
curl -X DELETE http://localhost:5284/api/units/2


# 3. Recipes

# a) Paged / List (JSON)

curl -X GET "http://localhost:5284/api/recipes?page=1&pageSize=10&search=pancake&sortBy=AvgRating&sortOrder=Desc" \
  -H "Accept: application/json"
  
# b) Get single with scale

curl -X GET "http://localhost:5284/api/recipes/5?scale=2.0" \
  -H "Accept: application/json"
  
# c) Create Recipe (form-data)

curl -X POST http://localhost:5284/api/recipes \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Name=Pancakes" \
  -F "DefaultQty=500,0" \
  -F "DefaultUnitId=1" \
  -F "DishType=Dessert" \
  -F "Description=Mix and cook" \
  -F "ImageFile=@/path/to/pancakes.jpg" \
  -F "Ingredients[0].IngredientId=1" \
  -F "Ingredients[0].Qty=300,0" \
  -F "Ingredients[0].UnitId=2" \
  -F "Ingredients[1].IngredientId=2" \
  -F "Ingredients[1].Qty=200,0" \
  -F "Ingredients[1].UnitId=1" \
  -F "Steps[0].StepOrder=1" \
  -F "Steps[0].TemplateText=Boil {Water}." \
  -F "Steps[0].ImageFile=@/path/to/step1.jpg" \
  -F "Steps[0].StepIngredients[0].IngredientId=2" \
  -F "Steps[0].StepIngredients[0].Fraction=1,0"
  
# d) Update Recipe (form-data)

curl -X PUT http://localhost:5284/api/recipes/5 \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Name=Banana Pancakes" \
  -F "DefaultQty=600,0" \
  -F "DefaultUnitId=1" \
  -F "DishType=Dessert" \
  -F "Description=Mix, add banana, and cook" \
  -F "ImageFile=@/path/to/banana.jpg" \
  -F "Ingredients[0].IngredientId=1" \
  -F "Ingredients[0].Qty=300,0" \
  -F "Ingredients[0].UnitId=2" \
  -F "Ingredients[1].IngredientId=3" \
  -F "Ingredients[1].Qty=100,0" \
  -F "Ingredients[1].UnitId=3" \
  -F "Steps[0].StepOrder=1" \
  -F "Steps[0].TemplateText=Mix {Flour} with {Milk}." \
  -F "Steps[0].ImageFile=@/path/to/step1-new.jpg" \
  -F "Steps[0].StepIngredients[0].IngredientId=2" \
  -F "Steps[0].StepIngredients[0].Fraction=1,0"
  
# e) Delete Recipe

curl -X DELETE http://localhost:5284/api/recipes/5


# 4. Recipe Ingredients (JSON)

# List
curl -X GET http://localhost:5284/api/recipes/5/recipeingredients \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/recipes/5/recipeingredients/9 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/recipes/5/recipeingredients \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"ingredientId":1,"qty":300.0,"unitId":2}'

# Update
curl -X PUT http://localhost:5284/api/recipes/5/recipeingredients/9 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"ingredientId":2,"qty":150.0,"unitId":1}'

# Delete
curl -X DELETE http://localhost:5284/api/recipes/5/recipeingredients/9


# 5. Instruction Steps (form-data)

# List
curl -X GET http://localhost:5284/api/recipes/5/instructionsteps \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/recipes/5/instructionsteps/14 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/recipes/5/instructionsteps \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "StepOrder=1" \
  -F "TemplateText=Boil the water until it steams." \
  -F "ImageFile=@/path/to/step1.jpg" \
  -F "StepIngredients[0].IngredientId=2" \
  -F "StepIngredients[0].Fraction=1,0"

# Update
curl -X PUT http://localhost:5284/api/recipes/5/instructionsteps/14 \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "StepOrder=1" \
  -F "TemplateText=Let it simmer for 5 minutes." \
  -F "ImageFile=@/path/to/step1-new.jpg" \
  -F "StepIngredients[0].IngredientId=2" \
  -F "StepIngredients[0].Fraction=1,0"

# Delete
curl -X DELETE http://localhost:5284/api/recipes/5/instructionsteps/14


6. Step Ingredients (JSON)

# List
curl -X GET http://localhost:5284/api/steps/14/stepingredients \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/steps/14/stepingredients/3 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/steps/14/stepingredients \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"ingredientId":2,"fraction":1.0}'

# Update
curl -X PUT http://localhost:5284/api/steps/14/stepingredients/3 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"ingredientId":3,"fraction":0.25}'

# Delete
curl -X DELETE http://localhost:5284/api/steps/14/stepingredients/3


7. Reviews (JSON)

# List
curl -X GET http://localhost:5284/api/recipes/5/reviews \
  -H "Accept: application/json"

# Get by id
curl -X GET http://localhost:5284/api/recipes/5/reviews/7 \
  -H "Accept: application/json"

# Create
curl -X POST http://localhost:5284/api/recipes/5/reviews \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"reviewer":"Alice","difficulty":2,"recommend":true,"comment":"Great!","rating":5,"preparationTimeMinutes":15}'

# Update
curl -X PUT http://localhost:5284/api/recipes/5/reviews/7 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  --data '{"reviewer":"Bob","rating":4,"preparationTimeMinutes":12}'

# Delete
curl -X DELETE http://localhost:5284/api/recipes/5/reviews/7
