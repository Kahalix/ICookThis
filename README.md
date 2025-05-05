# ICookThis
Food recipes and reviews web app ASP .Net Core Vue.js





Endpoints:

Create Recipe (import to Postman)

curl -X POST http://localhost:5284/api/recipes \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Name=Lody" \
  -F "DefaultQty=1,0" \
  -F "DefaultUnitId=1" \
  -F "DishType=Dessert" \
  -F "Description=Mix and cook" \
  -F "ImageFile=@/path/to/lody.jpg" \
  -F "Ingredients[0].IngredientId=1" \
  -F "Ingredients[0].Qty=300,0" \
  -F "Ingredients[0].UnitId=1" \
  -F "Ingredients[1].IngredientId=2" \
  -F "Ingredients[1].Qty=200,0" \
  -F "Ingredients[1].UnitId=2" \
  -F "Steps[0].StepOrder=1" \
  -F "Steps[0].TemplateText=Boil {Water}." \
  -F "Steps[0].StepIngredients[0].IngredientId=2" \
  -F "Steps[0].StepIngredients[0].Fraction=1,0"
  
Create InstructionStep (import to Postman)

curl -X POST http://localhost:5284/api/recipes/1/instructionsteps \
  -H "Accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "StepOrder=1" \
  -F "TemplateText=Boil the water until it steams." \
  -F "ImageFile=@/full/path/to/step1.jpg" \
  -F "StepIngredients[0].IngredientId=2" \
  -F "StepIngredients[0].Fraction=1,0"
