const colors = require('tailwindcss/colors')

module.exports = {
  mode: 'jit',
  // These paths are just examples, customize them to match your project structure
  purge: [
    '../**/*.{html,cshtml,razor}',
  ],
  darkMode: 'media', // or 'media' or 'class'
}
