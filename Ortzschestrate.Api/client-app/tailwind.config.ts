import type { Config } from "tailwindcss"
import defaultTheme from "tailwindcss/defaultTheme"

export default <Partial<Config>>{
  theme: {
    extend: {
      colors: {
        "pastel-green": {
          "50": "#f0fdf5",
          "100": "#dcfce8",
          "200": "#bbf7d1",
          "300": "#86efac",
          "400": "#4ade80",
          "500": "#22c55d",
          "600": "#16a349",
          "700": "#15803c",
          "800": "#166533",
          "900": "#14532b",
          "950": "#052e14",
        },
        shamrock: {
          DEFAULT: "#4ADE80",
          "50": "#E5FAED",
          "100": "#D4F7E1",
          "200": "#B2F1C9",
          "300": "#8FEBB0",
          "400": "#6DE498",
          "500": "#4ADE80",
          "600": "#25CB62",
          "700": "#1C9B4B",
          "800": "#146C34",
          "900": "#0B3D1D",
          "950": "#072512",
        },
        "oxford-blue": {
          DEFAULT: "#2F3745",
          50: "#627491",
          100: "#5C6D88",
          200: "#505E75",
          300: "#435063",
          400: "#374151",
          500: "#2F3745",
          600: "#272D39",
          700: "#1E242D",
          800: "#161A20",
          900: "#0E1014",
          950: "#0A0B0E",
        },
      },
    },
  },
}
