import base64
import binascii
import os
import tempfile

from fastapi import FastAPI, Form, HTTPException
from faster_whisper import WhisperModel

app = FastAPI()

model_cache = {}

CPU_THREADS = int(os.getenv("WHISPER_CPU_THREADS", "2"))

def get_model(model_name: str):
    allowed_models = {
        "tiny",
        "base",
        "small",
        "medium",
        "large-v2",
        "large-v3",
        "turbo",
    }

    if model_name not in allowed_models:
        raise HTTPException(
            status_code=400,
            detail=f"Unsupported model: {model_name}",
        )

    if model_name not in model_cache:
        model_cache[model_name] = WhisperModel(
            model_name,
            device="cpu",
            compute_type="int8",
	    cpu_threads=CPU_THREADS,
	    num_workers=CPU_THREADS,
        )

    return model_cache[model_name]


@app.post("/transcribe")
async def transcribe(
    model: str = Form("small"),
    url: str = Form(...),
):
    whisper_model = get_model(model)

    try:
        encoded_audio = url.strip()

        if encoded_audio.startswith("data:"):
            encoded_audio = encoded_audio.split(",", 1)[1]

        audio_bytes = base64.b64decode(
            encoded_audio,
            validate=True,
        )

    except (binascii.Error, ValueError):
        raise HTTPException(
            status_code=400,
            detail="url must contain valid Base64 audio",
        )

    if not audio_bytes:
        raise HTTPException(
            status_code=400,
            detail="Audio is empty",
        )

    temp_path = None

    try:
        with tempfile.NamedTemporaryFile(
            suffix=".ogg",
            delete=False,
        ) as temp_file:
            temp_file.write(audio_bytes)
            temp_path = temp_file.name

        segments, info = whisper_model.transcribe(
            temp_path,
            language="es",
            task="transcribe",
            vad_filter=True,
        )

        text = " ".join(
            segment.text.strip()
            for segment in segments
            if segment.text.strip()
        )

        return {
            "text": text,
        }

    except Exception as error:
        raise HTTPException(
            status_code=500,
            detail=f"Transcription failed: {error}",
        )

    finally:
        if temp_path and os.path.exists(temp_path):
            os.remove(temp_path)
