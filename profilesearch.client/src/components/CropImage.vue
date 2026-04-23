<script setup lang="js">
  import { store } from '../../store.js';
  import ImageSearch from './ImageSearch.vue';
</script>

<template>
  <div class="container">
    <label for="imageInput">Choose a profile picture:</label>

    <div>
      <input type="file" id="imageInput" name="imageInput" style="margin-bottom: 0.5rem" accept=".png, .jpg, .jpeg" @click="deactivateFields" @change="handleFileUpload" />

      <div :hidden="!isCanvasReady" style="padding: 0; margin-bottom: 0">
        <button @click="cropperActive = !cropperActive">Crop</button>
        <button>Resize</button>
        <button>Etc</button>
      </div>

      <!--<cropper-canvas class="image-preview" :hidden="!isCanvasReady">
        <cropper-image id="cropperImage" ref="cropperImage" cover rotatable scalable skewable translatable @transform="onCropperImageTransform"></cropper-image>-->

      <cropper-canvas v-if="isCanvasReady" ref="cropperCanvas" :class="store.isDarkMode ? 'image-preview-dark' : 'image-preview'" style="margin: 0">
        <cropper-image ref="cropperImage" alt="" rotatable scalable skewable @transform="onCropperImageTransform" />
        <cropper-shade hidden></cropper-shade>
        <cropper-handle action="select" plain></cropper-handle>
        <cropper-selection v-if="isSelectionReady" id="cropperSelection" aspect-ratio="1" ref="cropperSelection" dynamic movable resizable outlined @change="onCropperSelectionChange">
          <cropper-handle action="move" theme-color="rgba(255, 255, 255, 0.35)"></cropper-handle>
          <cropper-handle action="n-resize"></cropper-handle>
          <cropper-handle action="e-resize"></cropper-handle>
          <cropper-handle action="s-resize"></cropper-handle>
          <cropper-handle action="w-resize"></cropper-handle>
          <cropper-handle action="ne-resize"></cropper-handle>
          <cropper-handle action="nw-resize"></cropper-handle>
          <cropper-handle action="se-resize"></cropper-handle>
          <cropper-handle action="sw-resize"></cropper-handle>
        </cropper-selection>
      </cropper-canvas>

      <div style="display: flex" :hidden="!isCanvasReady">
        <button @click="saveImage">Save</button>
        <button @click="downloadImage">Download</button>
        <button @click="resetImage">Reset</button>
        <input type="range" id="zoom-slider" style="margin-left: auto; margin-right: 2.5%;" min="0" max="1" step="0.01" value="0" @change="zoomTo" />
      </div>

      <ImageSearch />
    </div>
  </div>
</template>

<script lang="js">
  import { defineComponent } from 'vue';
  import axios from 'axios';

  export default defineComponent({
    data() {
      return {
        cloudinaryContext: {},
        isCanvasReady: false,
        isSelectionReady: false,
        cropperActive: false,
        imageUrl: '',
        originalSize: { width: 0, height: 0 },
        selectionProperties: {
          x: -1,
          y: -1,
          width: 0,
          height: 0
        },
        prevZoom: 0.0,
      };
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    },
    methods: {
      async handleFileUpload(event) {
        if (event.target.value === "" || event.target.value == null) {
          this.isCanvasReady = false;
          this.isSelectionReady = false;
          this.imageUrl = '';
          this.originalSize = { width: 0, height: 0 };
          this.selectionProperties = {
            x: -1,
            y: -1,
            width: 0,
            height: 0
          }
          return;
        }

        let fileInput = document.getElementById('imageInput');

        this.imageUrl = URL.createObjectURL(event.target.files[0]);

        const imageRef = new Image();
        imageRef.src = this.imageUrl;
        await imageRef.decode();

        this.isCanvasReady = true;

        await this.$nextTick();

        let width = imageRef.width;
        let height = imageRef.height;

        if (imageRef.width <= 1080) {
          this.$refs.cropperCanvas.style.width = `${imageRef.width}px`
          this.$refs.cropperCanvas.style.height = `${imageRef.height}px`
        }
        else {
          const proportion = imageRef.width / imageRef.height;

          this.$refs.cropperCanvas.style.width = `1080px`
          this.$refs.cropperCanvas.style.height = `${1080 / proportion}px`

          width = 1080;
          height = 1080 / proportion;
        }

        await this.$nextTick();

        this.$refs.cropperImage.src = this.imageUrl;

        await new Promise(resolve => requestAnimationFrame(resolve))

        this.isSelectionReady = true;

        await this.$nextTick();

        const size = Math.min(width, height) * 0.8;
        const _x = (width - size) / 2;
        const _y = (height - size) / 2;

        this.$refs.cropperSelection.$change(_x, _y, size, size);

        this.originalSize = { width: imageRef.width, height: imageRef.height };

        this.selectionProperties = {
          x: _x,
          y: _y,
          width: size,
          height: size
        }

        // console.log("canvas", this.$refs.cropperCanvas);
        // console.log("selection", this.$refs.cropperSelection.$toCanvas());

        imageRef.src = "";
      },

      onCropperImageTransform(event) {
        const cropperCanvas = this.$refs.cropperCanvas;

        if (!cropperCanvas) {
          return;
        }

        const cropperImage = this.$refs.cropperImage;
        const cropperCanvasRect = cropperCanvas.getBoundingClientRect();

        // 1. Clone the cropper image.
        const cropperImageClone = cropperImage.cloneNode();

        // 2. Apply the new matrix to the cropper image clone.
        cropperImageClone.style.transform = `matrix(${event.detail.matrix.join(', ')})`;

        // 3. Make the cropper image clone invisible.
        cropperImageClone.style.opacity = '0';

        // 4. Append the cropper image clone to the cropper canvas.
        cropperCanvas.appendChild(cropperImageClone);

        // 5. Compute the boundaries of the cropper image clone.
        const cropperImageRect = cropperImageClone.getBoundingClientRect();

        // 6. Remove the cropper image clone.
        cropperCanvas.removeChild(cropperImageClone);

        if (this.notWithinBounds(cropperImageRect, cropperCanvasRect)) {
          event.preventDefault();
        }
      },

      onCropperSelectionChange(event) {
        const cropperCanvas = this.$refs.cropperCanvas;

        if (!cropperCanvas) {
          return;
        }

        const cropperCanvasRect = cropperCanvas.getBoundingClientRect();
        const selection = event.detail;

        const cropperImage = this.$refs.cropperImage;
        const cropperImageRect = cropperImage.getBoundingClientRect();
        const maxSelection = {
          x: cropperImageRect.left - cropperCanvasRect.left,
          y: cropperImageRect.top - cropperCanvasRect.top,
          width: cropperImageRect.width,
          height: cropperImageRect.height,
        };

        if (!this.inSelection(selection, maxSelection)) {
          event.preventDefault();
        }

        console.log("selectionnnn", selection, selection.x);
        this.selectionProperties = {
          x: selection.x,
          y: selection.y,
          width: selection.width,
          height: selection.height
        }
      },

      notWithinBounds(cropperImageRect, cropperCanvasRect) {
        return (
          cropperImageRect.top > cropperCanvasRect.top
          || cropperImageRect.right < cropperCanvasRect.right
          || cropperImageRect.bottom < cropperCanvasRect.bottom
          || cropperImageRect.left > cropperCanvasRect.left
        );
      },

      inSelection(selection, maxSelection) {
        return (
          selection.x >= maxSelection.x
          && selection.y >= maxSelection.y
          && (selection.x + selection.width) <= (maxSelection.x + maxSelection.width)
          && (selection.y + selection.height) <= (maxSelection.y + maxSelection.height)
        );
      },

      zoomTo(event) {
        const originalZoomRatio = parseFloat(event.target.value);

        let zoomRatio = originalZoomRatio;
        if (this.prevZoom > originalZoomRatio) {
          zoomRatio -= 1;
        }

        console.log(zoomRatio);

        const cropperImage = this.$refs.cropperImage;

        if (!cropperImage) {
          return;
        }

        if (this.originalSize.width < 600 || this.originalSize.height < 400) {
          event.preventDefault();
        } else {
          cropperImage.$zoom(zoomRatio);
          this.prevZoom = originalZoomRatio;
        }

      },

      async saveImage() {
        const image = this.$refs.cropperImage;

        const imageRef = new Image();
        imageRef.src = image.src;
        await imageRef.decode();

        if (!image || (this.selectionProperties.width == 0 && this.selectionProperties.height == 0)) {
          console.error("ERROR IN SAVEIMAGE - Invalid Image or Selection!");
          return;
        }

        const selection = this.$refs.cropperSelection;
        const croppedCanvas = await selection.$toCanvas();

        const blob = await new Promise(resolve => croppedCanvas.toBlob(resolve, 'image/png'));

        const cloudinaryUrl = await this.uploadToCloud(blob);

        let info = {
          UserId: 1,
          Name: 'Test'.toLowerCase(), // TODO: Eventually replace with using ML to determine character name and such
          Url: cloudinaryUrl,
          X: this.selectionProperties.x,
          Y: this.selectionProperties.y,
          Width: this.selectionProperties.width,
          Height: this.selectionProperties.height
        }

        let resp = axios.post(`/api/images`, info)
          .then(response => {
            console.log("saveImage returned", response);
          }).catch(e => {
            console.error("ERROR IN saveImage -", e);
          })
      },

      async downloadImage() {
        console.log("downloadImage");

        const selection = document.querySelector('cropper-selection');

        const canvas = await selection.$toCanvas();

        const link = document.createElement('a');
        link.download = this.isCanvasReady.name + "_cropped";
        link.href = canvas.toDataURL('image/png');
        link.click();
      },

      async resetImage() {
        const url = this.imageUrl;

        this.isCanvasReady = false;
        this.isSelectionReady = false;
        this.imageUrl = '';
        this.selectionProperties = {
          x: -1,
          y: -1,
          width: 0,
          height: 0
        }
        this.originalSize = { width: 0, height: 0 };
        document.getElementById("imageInput").value = null;

        this.$nextTick();

        this.handleFileUpload({ target: { value: null, files: [{ name: url }] } });
      },

      async uploadToCloud(imageBlob) {
        if (Object.entries(this.cloudinaryContext).length === 0) {
          this.cloudinaryContext = await this.getCloudinaryContext();
        }

        console.log("Uploading to Cloudinary...");

        const formData = new FormData();
        formData.append("file", imageBlob, "cropped-image.png");
        formData.append("upload_preset", this.cloudinaryContext.uploadPreset);

        try {
          const response = await fetch(
            this.cloudinaryContext.url,
            { method: "POST", body: formData },
          );

          if (!response.ok) {
            throw new Error(`Cloudinary upload failed: ${response.status} ${response.statusText}`);
          }

          const result = await response.json();
          console.log("Cloudinary upload successful:", result);

          // Return the secure URL of the uploaded image
          return result.secure_url;
        }
        catch (error) {
          console.error("ERROR uploading to Cloudinary:", error);
          throw error;
        }
      },

      async getCloudinaryContext() {
        return await fetch("/api/images/getCloudinaryContext")
          .then(response => {
            if (!response.ok) {
              throw new Error(`Failed to fetch Cloudinary context: ${response.status} ${response.statusText}`);
            }
            return response.json();
          })
          .then(data => {
            console.log("Cloudinary context fetched successfully:", data);
            return data;
          })
          .catch(error => {
            console.error("ERROR fetching Cloudinary context:", error);
            throw error;
          });
      },

      deactivateFields() {
        this.isCanvasReady = false;
        this.isSelectionReady = false;
      }
    },

    computed: {
      isDarkMode() {
        return this.$globalState.isDarkMode;
      }
    }
  });
</script>

<style scoped>
  .container {
    margin-bottom: 10px;
    padding: 1rem;
    border-radius: 10px;
  }

  .slider {
    justify-content: flex-end;
    width: 100px;
    height: 50px;
  }

  .image-preview {
    border: 3px dashed black;
  }

  .image-preview-dark {
    border: 3px dashed white;
  }
</style>

<!--<style lang="scss" scoped>
.cropper-container {
  border: 1px solid var(--vp-c-divider);
  border-radius: 0.375rem;
  margin-bottom: 1rem;
  margin-top: 1rem;
  padding: 1.25rem 1.5rem;

  :deep(cropper-canvas) {
    height: 360px;
  }
}
</style>-->
