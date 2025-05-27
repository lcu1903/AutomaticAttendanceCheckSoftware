from flask import Flask, request, jsonify
from deepface import DeepFace

app = Flask(__name__)

@app.route('/health', methods=['GET'])
def health():
    return "ok", 200

@app.route('/embedding', methods=['POST'])
def embedding():
    try:
        data = request.json
        print("Received data:", data)  # Log dữ liệu nhận được

        img = data['img']
        model_name = data.get('model', 'ArcFace')
        print("Model:", model_name)

        embedding = DeepFace.represent(img, model_name=model_name)[0]['embedding']
        print("Embedding calculated successfully")
        return jsonify({"embedding": embedding})
    except Exception as e:
        error_msg = f"Error in /embedding: {str(e)}\n"
        print(error_msg)
        # Ghi log lỗi ra file error_log.txt cùng thư mục
        with open("error_log.txt", "a", encoding="utf-8") as f:
            f.write(error_msg)
        return jsonify({"error": str(e)}), 500
@app.route('/verify', methods=['POST'])
def verify():
    data = request.json
    img1 = data['img1']
    img2 = data['img2']
    model_name = data.get('model', 'ArcFace')
    result = DeepFace.verify(img1, img2, model_name=model_name)
    return jsonify({"verified": result["verified"], "distance": result["distance"]})
@app.route('/routes', methods=['GET'])
def list_routes():
    import urllib
    output = []
    for rule in app.url_map.iter_rules():
        methods = ','.join(rule.methods)
        line = urllib.parse.unquote(f"{rule.endpoint}: {rule.rule} [{methods}]")
        output.append(line)
    return "<br>".join(output)
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5243)
    