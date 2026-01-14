// // components/sales/CartTable.js
// import React from "react";
// import { motion, AnimatePresence } from "framer-motion";
// import { Minus, Plus, Trash2 } from "lucide-react";

// export const CartTable = ({ 
//   items = [], 
//   onUpdateQuantity, 
//   onRemoveItem,
//   showHeader = true 
// }) => {
//   if (items.length === 0) {
//     return (
//       <div className="text-center py-12 text-gray-500 dark:text-gray-400">
//         <div className="text-6xl mb-4">🛒</div>
//         <p className="text-lg">No items in cart</p>
//         <p className="text-sm">Add products to get started</p>
//       </div>
//     );
//   }

//   return (
//     <div className="overflow-x-auto">
//       {showHeader && (
//         <div className="flex justify-between items-center mb-4">
//           <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100">
//             Sale Items • {items.length} item(s) in cart
//           </h2>
//         </div>
//       )}
      
//       <table className="w-full border-collapse">
//         <thead>
//           <tr className="border-b border-gray-200 dark:border-gray-600">
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">PRODUCT</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">AVAILABLE STOCK</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">QUANTITY</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">UNIT PRICE</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">LINE DISCOUNT</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">LINE TOTAL</th>
//             <th className="text-left p-3 font-semibold text-gray-900 dark:text-gray-100">ACTIONS</th>
//           </tr>
//         </thead>
//         <tbody>
//           <AnimatePresence mode="popLayout">
//             {items.map((item, index) => (
//               <motion.tr
//                 key={item.productId}
//                 initial={{ opacity: 0, y: 20 }}
//                 animate={{ opacity: 1, y: 0 }}
//                 exit={{ opacity: 0, y: -20 }}
//                 className="border-b border-gray-100 dark:border-gray-700"
//               >
//                 <td className="p-3 font-semibold text-gray-900 dark:text-gray-100">{item.name}</td>
//                 <td className="p-3 text-gray-600 dark:text-gray-400">
//                   {item.availableStock} – {item.availableStock - item.quantity}
//                 </td>
//                 <td className="p-3">
//                   <div className="flex items-center gap-2">
//                     <button
//                       onClick={() => onUpdateQuantity(item.productId, item.quantity - 1)}
//                       className="p-1 hover:bg-gray-100 dark:hover:bg-gray-600 rounded transition-colors"
//                     >
//                       <Minus size={16} />
//                     </button>
//                     <span className="px-3 py-1 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 min-w-[40px] text-center">
//                       {item.quantity}
//                     </span>
//                     <button
//                       onClick={() => onUpdateQuantity(item.productId, item.quantity + 1)}
//                       className="p-1 hover:bg-gray-100 dark:hover:bg-gray-600 rounded transition-colors"
//                     >
//                       <Plus size={16} />
//                     </button>
//                   </div>
//                 </td>
//                 <td className="p-3 text-gray-600 dark:text-gray-400">
//                   Rs. {item.unitPrice.toFixed(2)}
//                 </td>
//                 <td className="p-3 text-gray-600 dark:text-gray-400">
//                   Rs. {(item.lineDiscount || 0).toFixed(2)}
//                 </td>
//                 <td className="p-3 font-semibold text-gray-900 dark:text-gray-100">
//                   Rs. {item.lineTotal.toFixed(2)}
//                 </td>
//                 <td className="p-3">
//                   <button
//                     onClick={() => onRemoveItem(item.productId)}
//                     className="p-1 hover:bg-red-100 dark:hover:bg-red-900/20 rounded text-red-600 dark:text-red-400 transition-colors"
//                   >
//                     <Trash2 size={16} />
//                   </button>
//                 </td>
//               </motion.tr>
//             ))}
//           </AnimatePresence>
//         </tbody>
//       </table>
//     </div>
//   );
// };