<template>
  <div class="container">
    <label for="imageInput">Choose a profile picture:</label><br />

    <input type="file" id="imageInput" name="imageInput" accept=".png, .jpg, .jpeg" @change="handleFileUpload" /><br />

    <div :hidden="!imageEl">
      <button @click="cropperActive = !cropperActive">Crop</button>
      <button>Resize</button>
      <button>Etc</button><br />
    </div>

    <!--<cropper-canvas class="image-preview" :hidden="!imageEl">
      <cropper-image id="cropperImage" ref="cropperImage" cover rotatable scalable skewable translatable @transform="onCropperImageTransform"></cropper-image>-->

    <cropper-canvas v-if="imageEl" ref="cropperCanvas" class="image-preview">
      <cropper-image ref="cropperImage" alt="" rotatable scalable skewable translatable @transform="onCropperImageTransform" />
      <cropper-shade hidden></cropper-shade>
      <cropper-handle action="select" plain></cropper-handle>
      <cropper-selection v-if="selectionReady" id="cropperSelection" aspect-ratio="1" ref="cropperSelection" dynamic movable resizable outlined @change="onCropperSelectionChange">
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

    <div :hidden="!imageEl">
      <button>Save</button>
      <button @click="downloadImage">Download</button>
      <button>Reset</button>
    </div>
  </div>
</template>

<script lang="js">
  import { defineComponent } from 'vue';
  import Croppie from 'croppie';

  export default defineComponent({
    data() {
      return {
        imageEl: false,
        selectionReady: false,
        fileInput: null,
        cropperActive: false,
        loading: false,
        post: null
      };
    },
    async created() {
      // fetch the data when the view is created and the data is
      // already being observed
      //await this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    },
    methods: {
      async handleFileUpload(event) {
        if (event.target.value === "") {
          this.imageEl = false;
          this.selectionReady = false;
          this.fileInput = null;
          return;
        }

        this.fileInput = document.getElementById('imageInput');

        const imageUrl = URL.createObjectURL(event.target.files[0]);

        const imageRef = new Image();
        imageRef.src = imageUrl;
        await imageRef.decode();

        this.selectionReady = false;
        this.imageEl = true;

        await this.$nextTick();

        this.$refs.cropperCanvas.style.width = `${imageRef.width}px`
        this.$refs.cropperCanvas.style.height = `${imageRef.height}px`

        this.$refs.cropperImage.src = imageUrl;

        await new Promise(resolve => requestAnimationFrame(resolve))

        this.selectionReady = true;

        this.$refs.cropperSelection.$change(0, 0);

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

        if (!cropperCanvas || this.within === 'none') {
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

      async downloadImage() {
        console.log("downloadImage");

        const selection = document.querySelector('cropper-selection');

        const canvas = await selection.$toCanvas();

        const link = document.createElement('a');
        link.download = this.imageEl.name + "_cropped";
        link.href = canvas.toDataURL('image/png');
        link.click();
      }
    },
    computed: {

    }
  });
</script>

<style scoped>
  .container * {
    padding-bottom: 10px;
  }

  .image-preview {
    border: 3px solid black;
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
