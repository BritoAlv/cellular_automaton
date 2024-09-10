from flask import Flask, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

@app.route('/api/get', methods = ['GET'])
def register_user():
    response = [
        ['red', 'blue', 'blue', 'blue'],
        ['blue', 'red', 'blue', 'blue'],
        ['blue', 'blue', 'red', 'blue'],
        ['blue', 'blue', 'blue', 'red']
    ]
    return jsonify(response), 200
    
if __name__ == '__main__':
    app.run(debug=True, port=5000)