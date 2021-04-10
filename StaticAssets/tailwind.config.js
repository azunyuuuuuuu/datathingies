const colors = require('tailwindcss/colors')

module.exports = {
  mode: 'jit',
  // These paths are just examples, customize them to match your project structure
  purge: [
    './../**/*.{html,cshtml,razor}',
  ],
  darkMode: 'media', // or 'media' or 'class'
  theme: {
    extend: {
      colors: {
        'gray': colors.trueGray,
        'blue-gray': colors.blueGray,
        'cool-gray': colors.coolGray,
        'default-gray': colors.gray,
        'true-gray': colors.trueGray,
        'warm-gray': colors.warmGray,
        'red': colors.red,
        'orange': colors.orange,
        'amber': colors.amber,
        'yellow': colors.yellow,
        'lime': colors.lime,
        'green': colors.green,
        'emerald': colors.emerald,
        'teal': colors.teal,
        'cyan': colors.cyan,
        'light-blue': colors.lightBlue,
        'blue': colors.blue,
        'indigo': colors.indigo,
        'violet': colors.violet,
        'purple': colors.purple,
        'fuchsia': colors.fuchsia,
        'pink': colors.pink,
        'rose': colors.rose,
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
