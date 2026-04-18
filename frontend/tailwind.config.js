module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: "#f0f9ff",
          100: "#e0f2fe",
          200: "#bae6fd",
          300: "#7dd3fc",
          400: "#38bdf8",
          500: "#0ea5e9",
          600: "#0284c7",
          700: "#0369a1",
          800: "#075985",
          900: "#0c3d66",
        },
        medical: {
          50: "#faf8f3",
          100: "#f5f1e8",
          200: "#e8dcc8",
          300: "#dcc7a8",
          400: "#c9a870",
          500: "#b68a39",
          600: "#a37a2e",
          700: "#845d24",
          800: "#6b4a1a",
          900: "#553811",
        }
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
      }
    },
  },
  plugins: [],
}
