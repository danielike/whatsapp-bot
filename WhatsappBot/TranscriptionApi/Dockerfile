FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

COPY transcription_api.py .

EXPOSE 8083

CMD ["sh", "-c", "uvicorn transcription_api:app --host 0.0.0.0 --port ${API_PORT:-8083}"]
