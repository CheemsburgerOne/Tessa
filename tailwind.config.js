/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
      "./Components/**/*.{razor,html,cshtml}",
      "./node_modules/flowbite/**/*.js",
  ],
  theme: {
    extend: {},
  },
  plugins: [
      require('flowbite/plugin'),
  ], 
    
    // Disable preflight
    // daisyUI config (optional - here are the default values)
    // daisyui: {
    //     themes: ["light"], // false: only light + dark | true: all themes | array: specific themes like this ["light", "dark", "cupcake"]
    //     darkTheme: "light", // name of one of the included themes for dark mode// prefix for daisyUI classnames (components, modifiers and responsive class names. Not colors)
    //     logs: true, // Shows info about daisyUI version and used config in the console when building your CSS
    //     themeRoot: ":root", // The element that receives theme color CSS variables
    // },
}
