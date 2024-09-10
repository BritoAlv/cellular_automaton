from flask import Flask, Response, jsonify, request, send_file

app = Flask(__name__)

@app.route('/', methods = ['GET'])
def home():
    return send_file("./public/index.html")

@app.route('/index', methods = ['GET'])
def index():
    return send_file("./public/index.html")

@app.route('/armies', methods = ['GET'])
def armies():
    return send_file("./public/armies.html")

@app.route('/battle', methods = ['GET'])
def battle():
    return send_file("./public/battle.html")

@app.route('/obstacles', methods = ['GET'])
def obstacles():
    return send_file("./public/obstacles.html")

@app.route('/dimensions', methods = ['GET'])
def dimensions():
    return send_file("./public/dimensions.html")

if __name__ == '__main__':
    app.run(debug=True, port=5050)