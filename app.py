from flask import Flask, request, jsonify, render_template
from flask_sqlalchemy import SQLAlchemy
from flask_marshmallow import Marshmallow
from marshmallow_sqlalchemy import SQLAlchemyAutoSchema
from flask_cors import CORS

app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "*"}})

app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///products.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

db = SQLAlchemy(app)
ma = Marshmallow(app)


class Product(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(100), nullable=False)
    price = db.Column(db.Float, nullable=False)
    description = db.Column(db.String(255), nullable=False)
    image_url = db.Column(db.String(255), nullable=False)


class ProductSchema(SQLAlchemyAutoSchema):
    class Meta:
        model = Product
        load_instance = True

product_schema = ProductSchema()
products_schema = ProductSchema(many=True)


with app.app_context():
    db.create_all()
    
    
    if Product.query.count() == 0:
        products = [
            Product(
                name="Lodówka",
                price=3300,
                description="HISENSE RB470N4EFC1 No frost 200cm Czarna",
                image_url="https://prod-api.mediaexpert.pl/api/images/gallery_290_300/thumbnails/images/41/4153656/Lodowka-HISENSE-RB470N4EFC1-No-frost-200cm-Czarna-front-zamknieta.jpg"
            ),
            Product(
                name="Pralka",
                price=2690,
                description="ELECTROLUX MEW7F349PXP SteamCare 700 UniversalDose 9kg 1400 obr A",
                image_url="https://prod-api.mediaexpert.pl/api/images/gallery_290_300/thumbnails/images/58/5846172/Pralka-SAMSUNG-WW90CGC04DAB-front.jpg"
            )
        ]
        db.session.bulk_save_objects(products)
        db.session.commit()

# Endpointy CRUD
@app.route('/products', methods=['GET'])
def get_products():
    products = Product.query.all()
    return jsonify(products_schema.dump(products)), 200

@app.route('/products/<int:id>', methods=['GET'])
def get_product(id):
    product = Product.query.get_or_404(id)
    return jsonify(product_schema.dump(product)), 200

@app.route('/products', methods=['POST'])
def add_product():
    name = request.json['name']
    price = request.json['price']
    description = request.json['description']
    image_url = request.json.get('image_url', 'brak')
    
    new_product = Product(name=name, price=price, description=description, image_url=image_url)
    db.session.add(new_product)
    db.session.commit()
    
    return jsonify(product_schema.dump(new_product)), 201
 
@app.route('/products/<int:id>', methods=['PUT'])
def update_product(id):
    product = Product.query.get_or_404(id)
    
    product.name = request.json.get('name', product.name)
    product.price = request.json.get('price', product.price)
    product.description = request.json.get('description', product.description)
    product.image_url = request.json.get('image_url', product.image_url)
    
    db.session.commit()
    return jsonify(product_schema.dump(product)), 200

@app.route('/products/<int:id>', methods=['DELETE'])
def delete_product(id):
    product = Product.query.get_or_404(id)
    db.session.delete(product)
    db.session.commit()
    return '', 204

# Strona główna wyświetla produkty w HTML
@app.route('/', methods=['GET'])
def home():
    products = Product.query.all()
    return render_template('index.html', products=products)

if __name__ == '__main__':
    app.run(debug=True)
