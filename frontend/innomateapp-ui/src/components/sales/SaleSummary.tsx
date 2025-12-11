// // components/sales/SaleSummary.js
// import React from "react";
// import { DollarSign, CreditCard, Smartphone } from "lucide-react";

// export const SaleSummary = ({
//   subtotal = 0,
//   discount = 0,
//   tax = 0,
//   total = 0,
//   paidAmount = 0,
//   balance = 0,
//   paymentMethod = "cash",
//   saleType = "cash",
//   onPaymentMethodChange,
//   onSaleTypeChange,
//   onCompleteSale,
//   onCancel,
//   isSubmitting = false
// }) => {
//   const paymentMethods = [
//     { id: "cash", label: "Cash", icon: DollarSign },
//     { id: "card", label: "Card", icon: CreditCard },
//     { id: "mobile", label: "Mobile", icon: Smartphone },
//   ];

//   const saleTypes = [
//     { id: "cash", label: "Cash Sale" },
//     { id: "credit", label: "Credit Sale" },
//   ];

//   return (
//     <div className="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 p-4">
//       <div className="grid grid-cols-4 gap-6 mb-6">
//         {/* Left Summary */}
//         <div className="space-y-3">
//           <div>
//             <div className="text-sm text-gray-600 dark:text-gray-400">Subtotal</div>
//             <div className="text-xl font-bold text-gray-900 dark:text-gray-100">
//               Rs. {subtotal.toFixed(2)}
//             </div>
//           </div>
//           <div>
//             <div className="text-sm text-gray-600 dark:text-gray-400">Flat Discount</div>
//             <div className="text-lg font-semibold text-gray-900 dark:text-gray-100">
//               Rs. {discount.toFixed(2)}
//             </div>
//           </div>
//         </div>

//         {/* Middle Summary */}
//         <div className="space-y-3">
//           <div className="flex justify-between">
//             <span className="text-sm text-gray-600 dark:text-gray-400">Tax (10%)</span>
//             <span className="font-semibold text-gray-900 dark:text-gray-100">
//               Rs. {tax.toFixed(2)}
//             </span>
//           </div>
//           <div className="flex justify-between">
//             <span className="text-sm text-gray-600 dark:text-gray-400">Paid Amount</span>
//             <span className="font-semibold text-gray-900 dark:text-gray-100">
//               Rs. {paidAmount.toFixed(2)}
//             </span>
//           </div>
//         </div>

//         {/* Right Summary */}
//         <div className="space-y-3">
//           <div>
//             <div className="text-sm text-gray-600 dark:text-gray-400">Total</div>
//             <div className="text-2xl font-bold text-blue-600 dark:text-blue-400">
//               Rs. {total.toFixed(2)}
//             </div>
//           </div>
//           <div>
//             <div className="text-sm text-gray-600 dark:text-gray-400">Balance</div>
//             <div className="text-lg font-semibold text-orange-600 dark:text-orange-400">
//               Rs. {balance.toFixed(2)}
//             </div>
//           </div>
//         </div>

//         {/* Payment Methods */}
//         <div className="space-y-3">
//           <div className="text-sm text-gray-600 dark:text-gray-400 mb-2">Payment Methods</div>
//           <div className="flex gap-2">
//             {paymentMethods.map(method => (
//               <button
//                 key={method.id}
//                 onClick={() => onPaymentMethodChange(method.id)}
//                 className={`flex-1 py-2 px-3 rounded-lg border transition-colors ${
//                   paymentMethod === method.id
//                     ? 'border-blue-500 bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400'
//                     : 'border-gray-300 dark:border-gray-600 hover:border-gray-400 dark:hover:border-gray-500 text-gray-600 dark:text-gray-400'
//                 }`}
//               >
//                 <div className="flex items-center justify-center gap-1 text-sm font-medium">
//                   <method.icon size={16} />
//                   {method.label}
//                 </div>
//               </button>
//             ))}
//           </div>
//         </div>
//       </div>

//       {/* Action Buttons */}
//       <div className="flex justify-between items-center pt-4 border-t border-gray-200 dark:border-gray-600">
//         <div className="flex gap-2">
//           {saleTypes.map(type => (
//             <button
//               key={type.id}
//               onClick={() => onSaleTypeChange(type.id)}
//               className={`px-4 py-2 rounded-lg border transition-colors ${
//                 saleType === type.id
//                   ? type.id === 'credit'
//                     ? 'border-orange-500 bg-orange-50 dark:bg-orange-900/20 text-orange-600 dark:text-orange-400'
//                     : 'border-green-500 bg-green-50 dark:bg-green-900/20 text-green-600 dark:text-green-400'
//                   : 'border-gray-300 dark:border-gray-600 text-gray-600 dark:text-gray-400'
//               }`}
//             >
//               {type.label}
//             </button>
//           ))}
//         </div>

//         <div className="flex gap-3">
//           <button
//             onClick={onCancel}
//             className="px-6 py-3 border border-gray-300 dark:border-gray-600 rounded-lg font-semibold text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
//           >
//             Cancel
//           </button>
//           <button
//             onClick={onCompleteSale}
//             disabled={isSubmitting}
//             className="px-8 py-3 bg-green-600 hover:bg-green-700 disabled:bg-gray-300 dark:disabled:bg-gray-600 text-white rounded-lg font-bold text-lg transition-colors"
//           >
//             {saleType === 'credit' ? 'Complete Credit Sale' : 'Complete Sale'}
//           </button>
//         </div>
//       </div>

//       {/* Estimation Only Notice */}
//       <div className="text-center mt-4">
//         <span className="text-sm text-gray-500 dark:text-gray-400 italic">
//           Estimation Only
//         </span>
//       </div>
//     </div>
//   );
// };