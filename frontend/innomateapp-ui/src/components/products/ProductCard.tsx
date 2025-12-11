// // components/products/ProductCard.js
// import React from "react";
// import { motion } from "framer-motion";

// export const ProductCard = ({ 
//   product, 
//   onAddToCart, 
//   showStock = true,
//   variant = "default"
// }) => {
//   const { productId, name, defaultSalePrice, stockQuantity } = product;
//   const isOutOfStock = stockQuantity === 0;

//   const handleClick = () => {
//     if (!isOutOfStock && onAddToCart) {
//       onAddToCart(product);
//     }
//   };

//   const getVariantClasses = () => {
//     switch (variant) {
//       case "compact":
//         return `w-full text-left p-2 rounded-lg border transition-colors ${
//           isOutOfStock 
//             ? 'bg-gray-100 dark:bg-gray-700 border-gray-200 dark:border-gray-600 cursor-not-allowed' 
//             : 'bg-white dark:bg-gray-700 border-gray-200 dark:border-gray-600 hover:border-blue-500 hover:bg-blue-50 dark:hover:bg-blue-900/20'
//         }`;
//       default:
//         return `w-full flex items-center justify-between p-3 rounded-lg border transition-colors ${
//           isOutOfStock 
//             ? 'bg-gray-100 dark:bg-gray-700 border-gray-200 dark:border-gray-600 cursor-not-allowed' 
//             : 'bg-white dark:bg-gray-700 border-gray-200 dark:border-gray-600 hover:border-blue-500 hover:bg-blue-50 dark:hover:bg-blue-900/20'
//         }`;
//     }
//   };

//   return (
//     <motion.button
//       whileHover={!isOutOfStock ? { scale: 1.02 } : {}}
//       whileTap={!isOutOfStock ? { scale: 0.98 } : {}}
//       onClick={handleClick}
//       disabled={isOutOfStock}
//       className={getVariantClasses()}
//     >
//       <div className="text-left flex-1">
//         <div className={`font-semibold ${
//           isOutOfStock 
//             ? 'text-gray-400 dark:text-gray-500' 
//             : 'text-gray-900 dark:text-gray-100'
//         }`}>
//           {name}
//         </div>
//         <div className={`text-sm ${
//           isOutOfStock 
//             ? 'text-gray-400 dark:text-gray-500' 
//             : 'text-gray-600 dark:text-gray-400'
//         }`}>
//           Rs. {(defaultSalePrice || 0).toFixed(2)}
//           {showStock && ` • ${stockQuantity || 0} in stock`}
//         </div>
//       </div>
//       {isOutOfStock && (
//         <span className="text-xs text-red-500 font-medium ml-2">Out of stock</span>
//       )}
//     </motion.button>
//   );
// };

// // Quick search products component
// export const QuickSearchProducts = ({ products, onAddToCart }) => {
//   return (
//     <div className="space-y-2">
//       {products.map(product => (
//         <ProductCard
//           key={product.productId}
//           product={product}
//           onAddToCart={onAddToCart}
//           showStock={true}
//         />
//       ))}
//     </div>
//   );
// };