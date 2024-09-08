from flask import Flask, Response, jsonify, request, send_file

app = Flask(__name__)

@app.route('/', methods = ['GET'])
def home():
    return send_file("./public/index.html")

if __name__ == '__main__':
    app.run(debug=True, port=5050)