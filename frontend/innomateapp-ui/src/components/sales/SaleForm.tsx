// components/sales/SaleForm.tsx
import { useEffect, useState, useRef, useCallback } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { motion, AnimatePresence } from "framer-motion";
import {
  Search,
  Trash2,
  DollarSign,
  CreditCard,
  FileText,
  ShoppingCart,
  Plus,
  Minus,
  User,
  Check,
  X,
  AlertCircle,
  Package,
  MoreVertical,
  Monitor,
  Laptop,
  Mouse,
  Keyboard,
  Scan,
  Clock,
  Zap,
  Pause,
  Calendar,
  Percent
} from "lucide-react";
import type { SaleDTO, CreateSaleDTO, CreateSaleDetailDTO, CreatePaymentDTO } from "@/types/sale.js";
import { useToastStore } from "@/store/useToastStore.js";
import { useSaleStore } from "@/store/useSaleStore.js";
import { useSalesProducts, useProductActions } from "@/store/useProductStore.js";
import { useCustomers, useCustomerActions } from "@/store/usecustomerStore.js";
import { useTheme } from "@/hooks/useTheme.js";
import { CustomerDTO } from "@/types/customer.js";
import CustomerModal from '@/components/customer/CustomerModal.js';
import { useDebounce } from "@/hooks/useDebounce.js";

// Updated schema with tax handling
const saleSchema = z.object({
  customerId: z.number().int().min(1).optional(),
  saleDetails: z.array(
    z.object({
      productId: z.number().int().min(1, "Product required"),
      quantity: z.number().min(1, "Quantity must be greater than 0"),
      unitPrice: z.number().min(0, "Price must be >= 0"),
      discount: z.number().min(0).default(0),
      discountType: z.enum(['%', 'Rs']).default('%'),
    })
  ).min(1, "Add at least one product"),
  invoiceDiscount: z.number().min(0).default(0),
  invoiceDiscountType: z.enum(['%', 'Rs']).default('Rs'),
  saleType: z.enum(['cash', 'credit']).default('cash'),
  taxRate: z.number().min(0).max(100).default(0), // Tax rate in percentage
}).refine(
  (data) => data.saleType !== 'credit' || data.customerId !== undefined,
  {
    message: "Customer is required for credit sales",
    path: ["customerId"],
  }
);

export type FormValues = z.infer<typeof saleSchema>;

interface SaleFormProps {
  onShowInvoice?: (sale: SaleDTO) => void;
  onClose: (refresh?: boolean) => void;
}

// Helper component for displaying shortcuts
const ShortcutItem = ({ keys, action }: { keys: string[]; action: string }) => (
  <div className="flex items-center justify-between py-1">
    <span className="text-gray-700 dark:text-gray-300">{action}</span>
    <div className="flex gap-1">
      {keys.map((key, index) => (
        <span key={index}>
          <kbd className="px-2 py-1 text-xs font-semibold bg-gray-200 dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded shadow-sm">
            {key}
          </kbd>
          {index < keys.length - 1 && <span className="mx-1 text-gray-400">+</span>}
        </span>
      ))}
    </div>
  </div>
);

export default function SaleForm({ onClose, onShowInvoice }: SaleFormProps) {
  const toast = useToastStore(state => state.push);
  const { createSale } = useSaleStore();
  const salesProducts = useSalesProducts();
  const customers = useCustomers();
  const { fetchProductsForSales } = useProductActions();
  const { fetchCustomers } = useCustomerActions();

  const { isDark } = useTheme();

  const [searchQuery, setSearchQuery] = useState("");
  const debouncedSearchQuery = useDebounce(searchQuery, 300);
  const [selectedCustomer, setSelectedCustomer] = useState<CustomerDTO | null>(null);
  const [searchResults, setSearchResults] = useState<any[]>([]);
  const [showSearchResults, setShowSearchResults] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [discountType, setDiscountType] = useState<'line' | 'invoice'>('line');
  const [editingDiscount, setEditingDiscount] = useState<number | null>(null);
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [showAddCustomerModal, setShowAddCustomerModal] = useState(false);
  const [amountReceived, setAmountReceived] = useState('');
  const [highlightedIndex, setHighlightedIndex] = useState(0);
  const [focusedLineIndex, setFocusedLineIndex] = useState<number>(-1);
  const [focusedField, setFocusedField] = useState<'quantity' | 'discount'>('quantity');
  const [showHelp, setShowHelp] = useState(false);
  const [partialPayment, setPartialPayment] = useState(false);
  const [taxRate, setTaxRate] = useState(0); // Default tax rate
  const [invoiceNo, setInvoiceNo] = useState<string>('');

  const { getNextInvoiceNumber } = useSaleStore();

  useEffect(() => {
    getNextInvoiceNumber().then(inv => {
      if (inv) setInvoiceNo(inv);
    });
  }, []);

  const searchInputRef = useRef<HTMLInputElement>(null);
  const quantityInputRefs = useRef<(HTMLInputElement | null)[]>([]);
  const discountInputRefs = useRef<(HTMLInputElement | null)[]>([]);

  const {
    control,
    register,
    handleSubmit,
    watch,
    reset,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(saleSchema),
    defaultValues: {
      saleType: 'cash',
      saleDetails: [],
      invoiceDiscount: 0,
      invoiceDiscountType: 'Rs',
      taxRate: 0,
    },
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "saleDetails",
  });

  const saleDetails = watch("saleDetails") || [];
  const invoiceDiscount = watch("invoiceDiscount") || 0;
  const invoiceDiscountType = watch("invoiceDiscountType") || 'Rs';
  const saleType = watch("saleType");
  const formTaxRate = watch("taxRate") || 0;

  // Theme classes matching MainLayout
  const theme = {
    bg: isDark ? 'bg-gray-900' : 'bg-gray-100',
    bgSecondary: isDark ? 'bg-gray-800' : 'bg-white',
    text: isDark ? 'text-gray-200' : 'text-gray-800',
    textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
    border: isDark ? 'border-gray-700' : 'border-gray-200',
    borderDark: isDark ? 'border-gray-600' : 'border-gray-300',
    card: isDark ? 'bg-gray-800' : 'bg-white',
    cardHover: isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-50',
    input: isDark ? 'bg-gray-800' : 'bg-white',
    inputBorder: isDark ? 'border-gray-700' : 'border-gray-300',
    tableHeader: isDark ? 'bg-gray-800' : 'bg-gray-100',
    tableRow: isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-50',
    tableDivide: isDark ? 'divide-gray-700' : 'divide-gray-200',
  };

  // Load products on mount
  useEffect(() => {
    fetchProductsForSales();
    fetchCustomers();
  }, [fetchProductsForSales, fetchCustomers]);

  // Initialize ref arrays when fields change
  useEffect(() => {
    quantityInputRefs.current = quantityInputRefs.current.slice(0, fields.length);
    discountInputRefs.current = discountInputRefs.current.slice(0, fields.length);
  }, [fields.length]);

  // Calculations with Tax
  // Calculations
  const grossTotal = saleDetails.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);

  const calculateLineTotal = (item: any) => {
    const itemTotal = item.quantity * item.unitPrice;
    if (discountType === 'invoice') return itemTotal; // Ignore line discount if invoice discount is active

    if (item.discountType === '%') {
      return itemTotal - (itemTotal * (item.discount || 0) / 100);
    }
    return itemTotal - (item.discount || 0);
  };

  const totalLineDiscount = saleDetails.reduce((sum, item) => {
    const itemTotal = item.quantity * item.unitPrice;
    // Only calculate if line discount mode is active
    if (discountType !== 'line') return 0;

    if (item.discountType === '%') {
      return sum + (itemTotal * (item.discount || 0) / 100);
    }
    return sum + (item.discount || 0);
  }, 0);

  const invoiceDiscountAmount = discountType === 'invoice'
    ? (invoiceDiscountType === '%'
      ? (grossTotal * (invoiceDiscount || 0) / 100)
      : (invoiceDiscount || 0))
    : 0;

  const totalDiscount = totalLineDiscount + invoiceDiscountAmount;

  // Tax calculation on the amount after discounts
  const taxableAmount = grossTotal - totalDiscount;
  const taxAmount = taxableAmount * (formTaxRate / 100);

  const total = taxableAmount + taxAmount;

  // For UI compatibility, alias grossTotal as subtotal if that's what's used in render for "Subtotal" label
  const subtotal = grossTotal;

  const itemsCount = saleDetails.reduce((sum, item) => sum + item.quantity, 0);

  useEffect(() => {
    // Focus search input on mount
    searchInputRef.current?.focus();
  }, [customers]);

  // Update search results when query changes
  useEffect(() => {
    if (debouncedSearchQuery.trim() === "") {
      setSearchResults([]);
      setShowSearchResults(false);
      setHighlightedIndex(0);
    } else {
      const results = salesProducts.filter(product =>
        product.name.toLowerCase().includes(debouncedSearchQuery.toLowerCase()) ||
        product.sku?.toLowerCase().includes(debouncedSearchQuery.toLowerCase())
      );
      setSearchResults(results);
      setShowSearchResults(results.length > 0);
      setHighlightedIndex(0);
    }
  }, [debouncedSearchQuery, salesProducts]);

  // Enhanced Keyboard Manager
  useEffect(() => {
    const handleGlobalKeyboard = (e: KeyboardEvent) => {
      // Allow function keys and Escape even when in inputs
      if (e.target instanceof HTMLInputElement || e.target instanceof HTMLSelectElement || e.target instanceof HTMLTextAreaElement) {
        if ((e.target as HTMLInputElement).type === 'number' && (e.key === 'ArrowUp' || e.key === 'ArrowDown')) {
          return;
        }
        if (e.key === 'Escape') {
          e.preventDefault();
          (e.target as HTMLElement).blur();
          searchInputRef.current?.focus();
          return;
        }
        if (e.key === 'F1' || e.key === 'F2' || e.key === 'F3' || e.key === 'F5' || e.key === 'F6' || e.key === 'F8' || e.key === 'F9') {
          e.preventDefault();
        } else {
          return;
        }
      }

      switch (e.key) {
        case 'F2':
        case 'F3':
          e.preventDefault();
          searchInputRef.current?.focus();
          searchInputRef.current?.select();
          break;

        case 'F5':
          e.preventDefault();
          if (saleDetails.length > 0 && !isProcessing) {
            handleCheckout('cash');
          } else if (saleDetails.length === 0) {
            toast("Add products to cart first", 'warning');
          }
          break;

        case 'F6':
          e.preventDefault();
          if (saleDetails.length > 0 && !isProcessing) {
            handleCheckout('credit');
          } else if (saleDetails.length === 0) {
            toast("Add products to cart first", 'warning');
          }
          break;

        case 'F1':
          e.preventDefault();
          setShowHelp(prev => !prev);
          break;

        case 'F8':
          e.preventDefault();
          applyQuickDiscount(10);
          break;

        case 'F9':
          e.preventDefault();
          applyQuickDiscount(15);
          break;

        case '1':
        case '2':
        case '3':
          if (e.ctrlKey) {
            e.preventDefault();
            const customerIndex = parseInt(e.key) - 1;
            if (customers[customerIndex]) {
              setSelectedCustomer(customers[customerIndex]);
              setValue('customerId', customers[customerIndex].customerId);
              toast(`Selected customer: ${customers[customerIndex].name}`, 'success');
            }
          }
          break;

        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          if (e.altKey && focusedLineIndex >= 0) {
            e.preventDefault();
            const quantity = parseInt(e.key);
            updateQuantity(focusedLineIndex, quantity);
            toast(`Set quantity to ${quantity}`, 'success');
          }
          break;

        case '+':
        case '=':
          if (focusedLineIndex >= 0) {
            e.preventDefault();
            quickUpdateQuantity(focusedLineIndex, 1);
          }
          break;

        case '-':
        case '_':
          if (focusedLineIndex >= 0) {
            e.preventDefault();
            quickUpdateQuantity(focusedLineIndex, -1);
          }
          break;

        case 'Delete':
          if (e.ctrlKey && focusedLineIndex >= 0) {
            e.preventDefault();
            removeItem(focusedLineIndex);
          }
          break;

        case 'ArrowUp':
          // Navigate Cart Logic
          if (saleDetails.length > 0 && !showSearchResults) {
            e.preventDefault();
            if (focusedLineIndex > 0) {
              setFocusedLineIndex(focusedLineIndex - 1);
              setFocusedField('quantity');
            } else if (focusedLineIndex === -1) {
              // If nothing focused, focus last item
              setFocusedLineIndex(saleDetails.length - 1);
              setFocusedField('quantity');
            } else {
              // Cycle to bottom
              setFocusedLineIndex(saleDetails.length - 1);
              setFocusedField('quantity');
            }
          }
          break;

        case 'ArrowDown':
          // Navigate Cart Logic
          if (saleDetails.length > 0 && !showSearchResults) {
            e.preventDefault();
            if (focusedLineIndex < saleDetails.length - 1) {
              setFocusedLineIndex(focusedLineIndex + 1);
              setFocusedField('quantity');
            } else if (focusedLineIndex === saleDetails.length - 1) {
              // Cycle to top
              setFocusedLineIndex(0);
              setFocusedField('quantity');
            } else if (focusedLineIndex === -1) {
              // Focus first item
              setFocusedLineIndex(0);
              setFocusedField('quantity');
            }
          }
          break;

        case 'ArrowLeft':
          if (focusedLineIndex >= 0) {
            e.preventDefault();
            setFocusedField('quantity');
          }
          break;

        case 'ArrowRight':
          if (focusedLineIndex >= 0) {
            e.preventDefault();
            setFocusedField('discount');
          }
          break;

        case 'Escape':
          e.preventDefault();
          if (showSearchResults) {
            setSearchQuery('');
            setShowSearchResults(false);
          } else if (focusedLineIndex >= 0) {
            setFocusedLineIndex(-1);
            searchInputRef.current?.focus();
          } else {
            onClose();
          }
          break;
      }
    };

    window.addEventListener('keydown', handleGlobalKeyboard);
    return () => window.removeEventListener('keydown', handleGlobalKeyboard);
  }, [saleDetails.length, isProcessing, focusedLineIndex, saleType, selectedCustomer, toast, setValue, customers]);

  // Search Keyboard Navigation
  const handleSearchKeyDown = (e: React.KeyboardEvent) => {
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        setHighlightedIndex(prev =>
          prev < searchResults.length - 1 ? prev + 1 : prev
        );
        break;
      case 'ArrowUp':
        e.preventDefault();
        setHighlightedIndex(prev => prev > 0 ? prev - 1 : prev);
        break;
      case 'Enter':
        e.preventDefault();
        if (searchResults[highlightedIndex]) {
          addProductToCart(searchResults[highlightedIndex]);
        }
        break;
      case 'Tab':
        e.preventDefault();
        if (searchResults[highlightedIndex]) {
          addProductToCart(searchResults[highlightedIndex]);
        }
        break;
      case 'Escape':
        e.preventDefault();
        setSearchQuery('');
        setShowSearchResults(false);
        setHighlightedIndex(0);
        break;
    }
  };

  const addProductToCart = useCallback((product: any) => {
    if (product.availableStock === 0) {
      toast("Product is out of stock", 'warning');
      return;
    }
    const existingIndex = saleDetails.findIndex(item => item.productId === product.productId);

    if (existingIndex >= 0) {
      const currentQty = saleDetails[existingIndex].quantity;
      const newQty = currentQty + 1;

      if (newQty > product.availableStock) {
        toast(`Only ${product.availableStock} items available`, 'warning');
        return;
      }

      setValue(`saleDetails.${existingIndex}.quantity`, newQty);
      setFocusedLineIndex(existingIndex);
      setFocusedField('quantity');

      setTimeout(() => {
        quantityInputRefs.current[existingIndex]?.focus();
        quantityInputRefs.current[existingIndex]?.select();
      }, 50);

      toast(`Updated ${product.name} quantity to ${newQty}`, 'success');
    } else {
      append({
        productId: product.productId,
        quantity: 1,
        unitPrice: product.defaultSalePrice,
        discount: 0,
        discountType: '%',
      });
      const newIndex = saleDetails.length;
      setFocusedLineIndex(newIndex);
      setFocusedField('quantity');

      setTimeout(() => {
        quantityInputRefs.current[newIndex]?.focus();
        quantityInputRefs.current[newIndex]?.select();
      }, 50);

      toast(`Added ${product.name} to cart`, 'success');
    }

    setSearchQuery("");
    setShowSearchResults(false);
    setHighlightedIndex(0);
  }, [saleDetails, append, setValue, toast]);

  const handleCartKeyNavigation = (e: React.KeyboardEvent, index: number) => {
    switch (e.key) {
      case 'ArrowUp':
        if (!e.ctrlKey) {
          e.preventDefault();
          if (index > 0) {
            setFocusedLineIndex(index - 1);
            setFocusedField('quantity');
            setTimeout(() => quantityInputRefs.current[index - 1]?.focus(), 0);
          }
        }
        break;
      case 'ArrowDown':
        if (!e.ctrlKey) {
          e.preventDefault();
          if (index < saleDetails.length - 1) {
            setFocusedLineIndex(index + 1);
            setFocusedField('quantity');
            setTimeout(() => quantityInputRefs.current[index + 1]?.focus(), 0);
          }
        }
        break;
      case 'ArrowLeft':
        e.preventDefault();
        setFocusedField('quantity');
        break;
      case 'ArrowRight':
        e.preventDefault();
        setFocusedField('discount');
        break;
      case 'Enter':
        e.preventDefault();
        if (focusedField === 'quantity') {
          quantityInputRefs.current[index]?.select();
        } else {
          setEditingDiscount(index);
          setTimeout(() => {
            discountInputRefs.current[index]?.select();
          }, 0);
        }
        break;
      case 'Tab':
        e.preventDefault();
        if (e.shiftKey) {
          if (focusedField === 'quantity') {
            if (index > 0) {
              setFocusedLineIndex(index - 1);
              setFocusedField('discount');
            }
          } else {
            setFocusedField('quantity');
          }
        } else {
          if (focusedField === 'discount') {
            if (index < saleDetails.length - 1) {
              setFocusedLineIndex(index + 1);
              setFocusedField('quantity');
            }
          } else {
            setFocusedField('discount');
          }
        }
        break;
      case 'Delete':
        if (e.ctrlKey) {
          e.preventDefault();
          removeItem(index);
        }
        break;
      case '+':
      case '=':
        e.preventDefault();
        e.stopPropagation();
        quickUpdateQuantity(index, 1);
        break;
      case '-':
      case '_':
        e.preventDefault();
        e.stopPropagation();
        quickUpdateQuantity(index, -1);
        break;
    }
  };

  const updateQuantity = (index: number, newQuantity: number) => {
    // Determine product for stock check
    const product = salesProducts.find(p => p.productId === saleDetails[index].productId);

    // Check stock limit
    if (product && newQuantity > product.availableStock) {
      toast(`Only ${product.availableStock} items available`, 'warning');
      setValue(`saleDetails.${index}.quantity`, product.availableStock);
      return;
    }

    // Allow updates (even invalid ones like 0 temporarily while typing, though we prefer min 1)
    // For direct input, we'll clamp to 1 if it's completely invalid, but allow typing.
    // If strict 1, it's hard to clear. Let's rely on onBlur or just set it.
    // Actually, setting 0 is fine, form validation handles the rest (Cannot submit).
    // But to be user friendly, we'll default NaN to 0.
    setValue(`saleDetails.${index}.quantity`, newQuantity);
  };

  const quickUpdateQuantity = (index: number, delta: number) => {
    const currentQty = saleDetails[index].quantity || 0;
    const newQuantity = currentQty + delta;

    if (newQuantity < 1) {
      // Prevent removal via minus button, keep at 1 or do nothing
      return;
    }
    updateQuantity(index, newQuantity);
  };

  const updateDiscount = (index: number, discount: number, discountType?: '%' | 'Rs') => {
    if (discountType) {
      setValue(`saleDetails.${index}.discountType`, discountType);
    }

    const maxDiscount = saleDetails[index].quantity * saleDetails[index].unitPrice;
    if (discount > maxDiscount) {
      setValue(`saleDetails.${index}.discount`, maxDiscount);
      toast(`Discount capped at item total (Rs. ${maxDiscount.toFixed(2)})`, 'warning');
    } else {
      setValue(`saleDetails.${index}.discount`, Math.max(0, discount));
    }
  };

  const applyQuickDiscount = (percent: number) => {
    if (focusedLineIndex >= 0 && focusedLineIndex < saleDetails.length) {
      setValue(`saleDetails.${focusedLineIndex}.discount`, percent);
      setValue(`saleDetails.${focusedLineIndex}.discountType`, '%');
      toast(`Applied ${percent}% discount to item`, 'success');
    } else {
      toast("Select an item first to apply discount", 'warning');
    }
  };

  const removeItem = (index: number) => {
    const productName = salesProducts.find(p => p.productId === saleDetails[index].productId)?.name;
    remove(index);

    if (saleDetails.length === 1) {
      setFocusedLineIndex(-1);
    } else if (index === saleDetails.length - 1) {
      setFocusedLineIndex(index - 1);
    }

    toast(`Removed ${productName} from cart`, 'info');
  };

  const clearCart = () => {
    if (saleDetails.length === 0) {
      toast("Cart is already empty", 'info');
      return;
    }

    if (!confirm("Are you sure you want to clear the entire cart?")) {
      return;
    }

    reset({
      saleType: saleType,
      saleDetails: [],
      invoiceDiscount: 0,
      invoiceDiscountType: 'Rs',
      taxRate: formTaxRate,
      customerId: selectedCustomer?.customerId
    });
    setFocusedLineIndex(-1);
    toast("Cart cleared", 'success');
    searchInputRef.current?.focus();
  };

  const handleCheckout = (type: 'cash' | 'credit') => {
    if (saleDetails.length === 0) {
      toast("Add products to cart first", 'warning');
      return;
    }

    if (type === 'credit') {
      if (!selectedCustomer) {
        toast("Please select a customer for credit sales", 'warning');
        return;
      }
    }

    const outOfStockItems = saleDetails.filter(item => {
      const product = salesProducts.find(p => p.productId === item.productId);
      return product && item.quantity > product.availableStock;
    });

    if (outOfStockItems.length > 0) {
      const productNames = outOfStockItems.map(item =>
        salesProducts.find(p => p.productId === item.productId)?.name
      ).filter(Boolean).join(', ');
      toast(`Insufficient stock for: ${productNames}`, 'error');
      return;
    }

    setValue('saleType', type);
    setAmountReceived('');
    setPartialPayment(false);
    setShowPaymentModal(true);
  };

  const completeSale = async () => {
    setIsProcessing(true);

    try {
      const paidAmount = saleType === 'cash' ? Number(amountReceived || 0) : (partialPayment ? Number(amountReceived || 0) : 0);
      const balanceAmount = total - paidAmount;

      // Prepare sale details for API
      const saleDetailsPayload: CreateSaleDetailDTO[] = saleDetails.map(item => {
        const itemTotal = item.quantity * item.unitPrice;
        const discountAmount = item.discountType === '%'
          ? (itemTotal * (item.discount || 0) / 100)
          : (item.discount || 0);

        return {
          productId: item.productId,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
          discount: discountAmount,
          discountType: item.discountType,
          discountPercentage: item.discountType === '%' ? (item.discount || 0) : 0,
        };
      });

      // Prepare payments for API
      const paymentsPayload: CreatePaymentDTO[] = [];
      if (paidAmount > 0) {
        paymentsPayload.push({
          paymentMethod: saleType === 'cash' ? 'Cash' : 'Bank Transfer',
          amount: paidAmount,
          referenceNo: `INV-${Date.now()}`
        });
      }

      // Calculate subtotal for global discount
      const grossTotal = saleDetails.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);

      // Prepare main sale payload
      const payload: CreateSaleDTO = {
        customerId: selectedCustomer?.customerId ?? 0,
        invoiceNo: invoiceNo || `INV-${Date.now()}`,
        createdBy: 1, // This should come from auth context
        paidAmount: paidAmount,
        balanceAmount: balanceAmount,
        isFullyPaid: balanceAmount <= 0,
        discount: discountType === 'invoice'
          ? (invoiceDiscountType === '%' ? (grossTotal * (invoiceDiscount || 0) / 100) : (invoiceDiscount || 0))
          : 0,
        discountPercentage: discountType === 'invoice' && invoiceDiscountType === '%' ? (invoiceDiscount || 0) : 0,
        discountType: discountType === 'invoice' ? invoiceDiscountType : 'Rs',
        saleDetails: saleDetailsPayload,
        payments: paymentsPayload,
      };

      await createSale(payload);

      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));

      if (saleType === 'cash') {
        toast(`Cash sale completed successfully! Change: Rs. ${Math.max(0, paidAmount - total).toFixed(2)}`, 'success');
      } else if (partialPayment) {
        toast(`Credit sale with partial payment completed! Balance: Rs. ${balanceAmount.toFixed(2)}`, 'success');
      } else {
        toast(`Credit sale recorded! Total due: Rs. ${total.toFixed(2)}`, 'success');
      }

      // Reset form but keep sale type and tax rate
      reset({
        saleType: saleType,
        saleDetails: [],
        invoiceDiscount: 0,
        invoiceDiscountType: 'Rs',
        taxRate: formTaxRate,
        customerId: selectedCustomer?.customerId
      });
      setFocusedLineIndex(-1);

      setShowPaymentModal(false);

      // Fetch next invoice number for next sale
      getNextInvoiceNumber().then(inv => {
        if (inv) setInvoiceNo(inv);
      });
      onClose(true);
    } catch (err) {
      console.error("Error saving sale:", err);
      toast("Failed to save sale. Please try again.", 'error');
    } finally {
      setIsProcessing(false);
    }
  };

  const handleEstimate = () => {
    if (saleDetails.length === 0) {
      toast("Add products to cart first", 'warning');
      return;
    }

    // Prepare sale details for Preview
    const saleDetailsPayload: any[] = saleDetails.map((item) => ({
      productId: item.productId,
      productName: salesProducts.find(p => p.productId === item.productId)?.name || `product #${item.productId}`,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discount: item.discountType === '%'
        ? (item.quantity * item.unitPrice * (item.discount || 0) / 100)
        : (item.discount || 0),
      discountType: item.discountType,
      discountPercentage: item.discountType === '%' ? (item.discount || 0) : 0,
    }));

    // Calculate subtotal for global discount
    const grossTotal = saleDetails.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);

    const estimateSale: SaleDTO = {
      saleId: 0, // Temporary ID
      saleDate: new Date().toISOString(),
      invoiceNo: `EST-${Date.now()}`, // Temporary Estimate No
      totalAmount: total,
      createdBy: 1,
      createdAt: new Date().toISOString(),
      customerId: selectedCustomer?.customerId ?? 0,
      balanceAmount: total, // Full amount due
      isFullyPaid: false,
      paidAmount: 0,
      saleDetails: saleDetailsPayload,
      payments: [],
      saleType: 'estimate',
      customer: selectedCustomer || { customerId: 0, name: 'Walk-In Customer', phone: '', email: '', totalSales: 0, totalDue: 0, isActive: true }, // Ensure customer object exists
      discount: discountType === 'invoice'
        ? (invoiceDiscountType === '%' ? (grossTotal * (invoiceDiscount || 0) / 100) : (invoiceDiscount || 0))
        : 0,
      discountPercentage: discountType === 'invoice' && invoiceDiscountType === '%' ? (invoiceDiscount || 0) : 0,
      discountType: discountType === 'invoice' ? invoiceDiscountType : 'Rs',
      isEstimate: true // Flag for InvoicePreview
    };

    if (onShowInvoice) {
      onShowInvoice(estimateSale);
    } else {
      console.warn("onShowInvoice prop not provided to SaleForm");
    }
  };

  const changeReturn = saleType === 'cash' ? Math.max(0, Number(amountReceived || 0) - total) : 0;
  const creditBalance = saleType === 'credit' ? total - Number(amountReceived || 0) : 0;

  // Quick tax rate buttons
  const quickTaxRates = [0, 5, 8, 10, 13, 15, 18];

  return (
    <div className={`flex h-full ${theme.bg} ${theme.text} overflow-hidden`}>
      {/* Help Overlay */}
      <AnimatePresence>
        {showHelp && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black bg-opacity-75 z-50 flex items-center justify-center"
            onClick={() => setShowHelp(false)}
          >
            <motion.div
              initial={{ scale: 0.9, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              className="bg-white dark:bg-gray-800 rounded-lg p-6 max-w-4xl w-full mx-4 max-h-[80vh] overflow-y-auto"
              onClick={e => e.stopPropagation()}
            >
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold text-gray-900 dark:text-white">Keyboard Shortcuts</h2>
                <button
                  onClick={() => setShowHelp(false)}
                  className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 p-2"
                >
                  <X size={24} />
                </button>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-blue-600 dark:text-blue-400 border-b border-gray-200 dark:border-gray-700 pb-2">
                    Navigation
                  </h3>
                  <div className="space-y-2">
                    <ShortcutItem keys={['F2', 'F3']} action="Focus Search" />
                    <ShortcutItem keys={['↑', '↓']} action="Navigate Products (Search)" />
                    <ShortcutItem keys={['↑', '↓']} action="Navigate Cart (When Active)" />
                    <ShortcutItem keys={['←', '→']} action="Switch Fields" />
                  </div>
                </div>

                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-green-600 dark:text-green-400 border-b border-gray-200 dark:border-gray-700 pb-2">
                    Sales
                  </h3>
                  <div className="space-y-2">
                    <ShortcutItem keys={['F5']} action="Cash Sale" />
                    <ShortcutItem keys={['F6']} action="Credit Sale" />
                    <ShortcutItem keys={['F8', 'F9']} action="Quick Discount" />
                    <ShortcutItem keys={['Alt', '1-9']} action="Quick Quantity" />
                  </div>
                </div>

                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-orange-600 dark:text-orange-400 border-b border-gray-200 dark:border-gray-700 pb-2">
                    Quick Actions
                  </h3>
                  <div className="space-y-2">
                    <ShortcutItem keys={['Ctrl', '1-3']} action="Quick Customer" />
                    <ShortcutItem keys={['+', '-']} action="Adjust Quantity" />
                    <ShortcutItem keys={['Ctrl', 'Delete']} action="Remove Item" />
                    <ShortcutItem keys={['F1']} action="This Help" />
                  </div>
                </div>

                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-purple-600 dark:text-purple-400 border-b border-gray-200 dark:border-gray-700 pb-2">
                    In Cart
                  </h3>
                  <div className="space-y-2">
                    <ShortcutItem keys={['Enter']} action="Edit Field" />
                    <ShortcutItem keys={['Tab']} action="Next Field" />
                    <ShortcutItem keys={['Shift', 'Tab']} action="Previous Field" />
                    <ShortcutItem keys={['Esc']} action="Back to Search" />
                  </div>
                </div>
              </div>

              <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
                <p className="text-sm text-blue-700 dark:text-blue-300">
                  💡 <strong>Pro Tip:</strong> Most shortcuts work globally. Keep your hands on the keyboard for maximum efficiency!
                </p>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Main Content Area */}
      <div className="flex-1 flex overflow-hidden">
        {/* Left Section - Customer & Products */}
        <div className={`w-[45%] flex flex-col ${theme.bg} border-r ${theme.border} overflow-hidden`}>
          {/* Header */}
          <div className={`p-4 border-b ${theme.border}`}>
            <div className="flex items-center justify-between mb-3">
              <h1 className={`text-xl font-bold ${theme.text}`}>SmartOps POS</h1>
              <button
                onClick={() => setShowHelp(true)}
                className="flex items-center gap-2 px-3 py-1 bg-gray-200 dark:bg-gray-700 rounded-lg text-sm hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors"
                title="Show Keyboard Shortcuts (F1)"
              >
                <Keyboard size={16} />
                <span>Help</span>
              </button>
            </div>

            {/* Sale Type & Customer Selection */}
            <div className="flex gap-3 mb-3">
              <div className="flex-1">
                <label className={`text-xs ${theme.textSecondary} mb-1 block`}>Sale Type</label>
                <div className="flex bg-gray-200 dark:bg-gray-700 rounded-lg p-1">
                  <button
                    onClick={() => {
                      setValue('saleType', 'cash');
                      setSelectedCustomer(null);
                      setValue('customerId', undefined);
                    }}
                    className={`flex-1 px-3 py-2 rounded text-sm font-medium transition-colors ${saleType === 'cash'
                      ? 'bg-white dark:bg-gray-600 shadow text-blue-600 dark:text-blue-400'
                      : 'text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200'
                      }`}
                  >
                    Cash Sale
                  </button>
                  <button
                    onClick={() => {
                      setValue('saleType', 'credit');
                      if (!selectedCustomer || selectedCustomer.customerId === 1) {
                        const firstRealCustomer = customers.find(c => c.customerId !== 1);
                        if (firstRealCustomer) {
                          setSelectedCustomer(firstRealCustomer);
                          setValue('customerId', firstRealCustomer.customerId);
                        }
                      }
                    }}
                    className={`flex-1 px-3 py-2 rounded text-sm font-medium transition-colors ${saleType === 'credit'
                      ? 'bg-white dark:bg-gray-600 shadow text-orange-600 dark:text-orange-400'
                      : 'text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200'
                      }`}
                  >
                    Credit Sale
                  </button>
                </div>
              </div>

              <div className="flex-1">
                <div className="flex justify-between items-center mb-1">
                  <label className={`text-xs ${theme.textSecondary}`}>
                    Customer {saleType === 'credit' && '*'}
                  </label>
                  <button
                    onClick={() => setShowAddCustomerModal(true)}
                    className="text-blue-500 hover:text-blue-700 text-xs flex items-center gap-1 font-medium bg-blue-50 dark:bg-blue-900/20 px-1.5 py-0.5 rounded transition-colors"
                    title="Add New Customer"
                    tabIndex={-1}
                  >
                    <Plus size={12} />
                    New
                  </button>
                </div>
                <div className={`flex items-center ${theme.input} rounded-lg px-3 py-2 border ${theme.inputBorder}`}>
                  <User size={16} className={theme.textSecondary + " mr-2"} />
                  <select
                    value={selectedCustomer?.customerId || ''}
                    onChange={(e) => {
                      const customer = customers.find(c => c.customerId === parseInt(e.target.value));
                      setSelectedCustomer(customer || null);
                      setValue('customerId', customer?.customerId);
                    }}
                    className={`flex-1 bg-transparent text-sm ${theme.text} outline-none`}
                  >
                    <option value="">Walk-In (Default)</option>
                    {customers.map(customer => (
                      <option key={customer.customerId} value={customer.customerId}>
                        {customer.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>

            {/* Tax Rate Selection */}
            <div className="mb-3">
              <label className={`text-xs ${theme.textSecondary} mb-1 block`}>Tax Rate</label>
              <div className="flex items-center gap-2">
                <div className="flex-1 flex gap-1">
                  {quickTaxRates.map(rate => (
                    <button
                      key={rate}
                      type="button"
                      onClick={() => setValue('taxRate', rate)}
                      className={`flex-1 px-2 py-1 text-xs rounded transition-colors ${formTaxRate === rate
                        ? 'bg-blue-500 text-white'
                        : 'bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-gray-600'
                        }`}
                    >
                      {rate}%
                    </button>
                  ))}
                </div>
                <div className="w-20">
                  <input
                    type="number"
                    step="1"
                    min="0"
                    max="100"
                    {...register('taxRate', { valueAsNumber: true })}
                    className={`w-full ${theme.input} text-xs ${theme.text} px-2 py-1 rounded border ${theme.inputBorder} outline-none`}
                    placeholder="Custom %"
                  />
                </div>
              </div>
            </div>

            {/* Search Products */}
            <div className="relative">
              <Search className={`absolute left-3 top-2.5 ${theme.textSecondary}`} size={16} />
              <input
                ref={searchInputRef}
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={handleSearchKeyDown}
                onFocus={() => setShowSearchResults(true)}
                placeholder="Search products (F2) or scan barcode..."
                className={`w-full ${theme.input} text-sm rounded-lg pl-9 pr-4 py-2 ${theme.text} placeholder-gray-500 border ${theme.inputBorder} focus:border-blue-500 outline-none`}
              />
            </div>
          </div>

          {/* Product List */}
          <div className="flex-1 p-4 overflow-y-auto" style={{ scrollbarWidth: 'none', msOverflowStyle: 'none' }}>
            <style>{`.flex-1::-webkit-scrollbar { display: none; }`}</style>

            {/* Search Results */}
            {showSearchResults && searchResults.length > 0 && (
              <div className="mb-4">
                <div className={`text-xs ${theme.textSecondary} mb-2 flex justify-between`}>
                  <span>Search Results ({searchResults.length})</span>
                  <span>Press ↑↓ to navigate, Enter to add</span>
                </div>
                <div className="grid gap-2">
                  {searchResults.slice(0, 50).map((product, index) => {
                    const ProductIcon = Package;
                    const isOutOfStock = product.availableStock === 0;
                    const isLowStock = product.availableStock < product.reorderLevel;

                    return (
                      <div
                        key={product.productId}
                        onClick={() => !isOutOfStock && addProductToCart(product)}
                        className={`${theme.card} rounded-lg p-3 cursor-pointer transition-all border ${highlightedIndex === index
                          ? 'border-blue-500 bg-blue-500 bg-opacity-10 shadow-lg'
                          : theme.borderDark + ' ' + theme.cardHover
                          } ${isOutOfStock ? 'opacity-60 cursor-not-allowed' : ''}`}
                      >
                        <div className="flex items-center justify-between">
                          <div className="flex items-center space-x-3 flex-1">
                            <div className={`w-8 h-8 flex-shrink-0 rounded flex items-center justify-center ${isOutOfStock
                              ? 'bg-gray-100 dark:bg-gray-900'
                              : 'bg-blue-100 dark:bg-blue-900'
                              }`}>
                              <ProductIcon
                                className={isOutOfStock ? 'text-gray-500 dark:text-gray-400' : 'text-blue-500'}
                                size={16}
                              />
                            </div>
                            <div className="flex-1 min-w-0">
                              <h3 className={`text-sm font-medium ${theme.text} truncate`}>{product.name}</h3>
                              <p className={`text-xs ${theme.textSecondary} mt-0.5`}>{product.sku} • {product.categoryName}</p>
                            </div>
                          </div>
                          <div className="text-right ml-4 flex-shrink-0">
                            <p className={`text-sm font-semibold ${theme.text}`}>Rs. {product.defaultSalePrice.toFixed(2)}</p>
                            <p className={`text-xs ${isOutOfStock
                              ? 'text-red-400'
                              : isLowStock
                                ? 'text-orange-400'
                                : 'text-green-400'
                              }`}>
                              {isOutOfStock ? 'Out of Stock' : isLowStock ? `Low: ${product.availableStock}` : `${product.availableStock} in stock`}
                            </p>
                          </div>
                        </div>
                        {highlightedIndex === index && (
                          <p className="text-xs text-blue-600 mt-2 flex items-center gap-1">
                            <Zap size={12} />
                            Press Enter to add to cart
                          </p>
                        )}
                      </div>
                    );
                  })}
                </div>
              </div>
            )}

            {/* All Products */}
            {!searchQuery && (
              <div className="grid gap-2">
                {salesProducts.slice(0, 50).map(product => {
                  const ProductIcon = Package;
                  const isOutOfStock = product.availableStock === 0;
                  const isLowStock = product.availableStock < product.reorderLevel;
                  return (
                    <div
                      key={product.productId}
                      onClick={() => !isOutOfStock && addProductToCart(product)}
                      className={`${theme.card} rounded-lg p-3 cursor-pointer transition-all border ${theme.borderDark + ' ' + theme.cardHover
                        } ${isOutOfStock ? 'opacity-60 cursor-not-allowed' : ''}`}
                    >
                      <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-3 flex-1">
                          <div className={`w-8 h-8 flex-shrink-0 rounded flex items-center justify-center ${isOutOfStock
                            ? 'bg-gray-100 dark:bg-gray-900'
                            : 'bg-blue-100 dark:bg-blue-900'
                            }`}>
                            <ProductIcon
                              className={isOutOfStock ? 'text-gray-500 dark:text-gray-400' : 'text-blue-500'}
                              size={16}
                            />
                          </div>
                          <div className="flex-1 min-w-0">
                            <h3 className={`text-sm font-medium ${theme.text} truncate`}>{product.name}</h3>
                            <p className={`text-xs ${theme.textSecondary} mt-0.5`}>{product.sku} • {product.categoryName}</p>
                          </div>
                        </div>
                        <div className="text-right ml-4 flex-shrink-0">
                          <p className={`text-sm font-semibold ${theme.text}`}>Rs. {product.defaultSalePrice.toFixed(2)}</p>
                          <p className={`text-xs ${isOutOfStock
                            ? 'text-red-400'
                            : isLowStock
                              ? 'text-orange-400'
                              : 'text-green-400'
                            }`}>
                            {isOutOfStock ? 'Out of Stock' : isLowStock ? `Low: ${product.availableStock}` : `${product.availableStock} in stock`}
                          </p>
                        </div>
                      </div>
                    </div>
                  );
                })}
                {salesProducts.length > 50 && (
                  <div className={`text-center py-4 text-xs ${theme.textSecondary}`}>
                    Showing top 50 items. Use search to find more.
                  </div>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Right Section - Cart & Checkout */}
        <div className={`w-[55%] flex flex-col ${theme.bgSecondary} overflow-hidden`}>
          {/* Current Sale Header */}
          <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-5 py-2.5 flex items-center justify-between">
            <div>
              <h2 className="text-base font-bold text-white">Current Sale</h2>
              <p className="text-xs text-blue-100">
                {invoiceNo} • {selectedCustomer?.name || 'No Customer'} • {saleType.toUpperCase()} SALE
                {formTaxRate > 0 && ` • Tax: ${formTaxRate}%`}
              </p>
            </div>
            <div className="text-right">
              <p className="text-2xl font-bold text-white">Rs. {total.toFixed(2)}</p>
              <p className="text-xs text-blue-100">{saleDetails.length} items • {itemsCount} units</p>
            </div>
          </div>

          {/* Line Items Table */}
          <div className="flex-1 overflow-y-auto" style={{ scrollbarWidth: 'none', msOverflowStyle: 'none' }}>
            <style>{`.flex-1::-webkit-scrollbar { display: none; }`}</style>
            {saleDetails.length === 0 ? (
              <div className={`flex items-center justify-center h-full ${theme.textSecondary}`}>
                <div className="text-center">
                  <ShoppingCart size={48} className="mx-auto mb-2 opacity-50" />
                  <p>Cart is empty. Add products to start.</p>
                  <p className="text-sm mt-1">Press F2 to focus search</p>
                </div>
              </div>
            ) : (
              <table className="w-full">
                <thead className={`${theme.tableHeader} sticky top-0 z-10`}>
                  <tr className={`text-xs ${theme.textSecondary} border-b ${theme.border}`}>
                    <th className="text-left py-2 px-3 font-medium">Product</th>
                    <th className="text-center py-2 px-2 font-medium w-16">Stock</th>
                    <th className="text-center py-2 px-2 font-medium w-32">Quantity</th>
                    <th className="text-right py-2 px-3 font-medium w-20">Price</th>
                    <th className="text-center py-2 px-2 font-medium w-20">Disc%</th>
                    <th className="text-right py-2 px-3 font-medium w-28">Total</th>
                    <th className="text-center py-2 px-2 font-medium w-10"></th>
                  </tr>
                </thead>
                <tbody className={`${theme.tableDivide} divide-y`}>
                  {fields.map((field, index) => {
                    const item = saleDetails[index];
                    const product = salesProducts.find(p => p.productId === item.productId);
                    const ProductIcon = Package;
                    const lineTotal = calculateLineTotal(item);
                    const isOutOfStock = product ? item.quantity > product.availableStock : false;
                    const remainingStock = product ? product.availableStock - item.quantity : 0;
                    const isFocused = focusedLineIndex === index;

                    return (
                      <tr
                        key={field.id}
                        className={`${theme.tableRow} transition-colors ${isFocused ? 'bg-blue-50 dark:bg-blue-900/20' : ''
                          }`}
                      >
                        <td className="py-2 px-3">
                          <div className="flex items-center space-x-2">
                            <div className="w-6 h-6 flex-shrink-0 bg-blue-100 dark:bg-blue-900 rounded flex items-center justify-center">
                              <ProductIcon className="text-blue-500" size={14} />
                            </div>
                            <div>
                              <p className={`text-sm font-medium ${theme.text} leading-tight`}>{product?.name}</p>
                              <p className={`text-xs ${theme.textSecondary} mt-0.5`}>{product?.sku}</p>
                            </div>
                          </div>
                        </td>
                        <td className="py-2 px-2 text-center">
                          <span className={`text-xs px-1.5 py-0.5 rounded ${isOutOfStock ? 'bg-red-900 text-red-200' :
                            remainingStock < 5 ? 'bg-orange-900 text-orange-200' : 'bg-green-900 text-green-200'
                            }`}>
                            {product?.availableStock || 0}
                          </span>
                        </td>
                        <td className="py-2 px-2">
                          <div
                            className={`flex items-center justify-center space-x-1.5 p-1 rounded ${isFocused && focusedField === 'quantity' ? 'bg-white dark:bg-gray-700 shadow-sm border border-blue-200' : ''
                              }`}
                            onKeyDown={(e) => handleCartKeyNavigation(e, index)}
                            tabIndex={0}
                          >
                            <button
                              type="button"
                              onClick={(e) => {
                                e.stopPropagation();
                                quickUpdateQuantity(index, -1);
                              }}
                              className={`w-6 h-6 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} rounded flex items-center justify-center transition-colors`}
                            >
                              <Minus size={12} />
                            </button>
                            <input
                              ref={el => {
                                quantityInputRefs.current[index] = el;
                              }}
                              type="number"
                              value={item.quantity || ''}
                              onChange={(e) => {
                                const val = parseInt(e.target.value);
                                updateQuantity(index, isNaN(val) ? 0 : val);
                              }}
                              onBlur={(e) => {
                                const val = parseInt(e.target.value);
                                if (isNaN(val) || val < 1) {
                                  updateQuantity(index, 1);
                                }
                              }}
                              onKeyDown={(e) => {
                                handleCartKeyNavigation(e, index);
                              }}
                              onFocus={(e) => {
                                e.target.select();
                                setFocusedLineIndex(index);
                                setFocusedField('quantity');
                              }}
                              className={`w-10 text-center text-sm font-medium ${theme.input} border ${theme.inputBorder} rounded outline-none`}
                            />
                            <button
                              type="button"
                              onClick={(e) => {
                                e.stopPropagation();
                                quickUpdateQuantity(index, 1);
                              }}
                              className={`w-6 h-6 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} rounded flex items-center justify-center transition-colors`}
                            >
                              <Plus size={12} />
                            </button>
                          </div>
                        </td>
                        <td className={`py-2 px-3 text-right text-sm ${theme.textSecondary}`}>
                          {item.unitPrice.toFixed(2)}
                        </td>
                        <td className="py-2 px-2">
                          <div
                            className={`p-1 rounded ${discountType === 'line' && isFocused && focusedField === 'discount'
                              ? 'bg-white dark:bg-gray-700 shadow-sm border border-blue-200'
                              : ''
                              }`}
                            onKeyDown={(e) => discountType === 'line' && handleCartKeyNavigation(e, index)}
                            tabIndex={discountType === 'line' ? 0 : -1}
                          >
                            {discountType === 'line' ? (
                              editingDiscount === index ? (
                                <div
                                  className="flex items-center space-x-1"
                                  onBlur={(e) => {
                                    // Only stop editing if focus moves OUTSIDE the container
                                    if (!e.currentTarget.contains(e.relatedTarget as Node)) {
                                      setEditingDiscount(null);
                                    }
                                  }}
                                >
                                  <input
                                    ref={el => {
                                      discountInputRefs.current[index] = el;
                                    }}
                                    type="number"
                                    value={saleDetails[index]?.discount ?? ''}
                                    onChange={(e) => {
                                      const val = e.target.value === '' ? 0 : parseFloat(e.target.value);
                                      updateDiscount(index, isNaN(val) ? 0 : val);
                                    }}
                                    className={`w-12 ${isDark ? 'bg-gray-700' : 'bg-gray-200'} text-center text-sm rounded px-1 py-0.5 outline-none`}
                                    autoFocus
                                    onFocus={(e) => e.target.select()}
                                  />
                                  <select
                                    value={saleDetails[index]?.discountType}
                                    onChange={(e) => updateDiscount(index, saleDetails[index].discount, e.target.value as '%' | 'Rs')}
                                    className={`text-xs ${isDark ? 'bg-gray-700' : 'bg-gray-200'} rounded px-1 outline-none`}
                                  >
                                    <option value="%">%</option>
                                    <option value="Rs">Rs</option>
                                  </select>
                                </div>
                              ) : (
                                <button
                                  onClick={() => {
                                    setEditingDiscount(index);
                                    setFocusedLineIndex(index);
                                    setFocusedField('discount');
                                  }}
                                  className={`w-full text-sm ${theme.textSecondary} hover:text-blue-400`}
                                >
                                  {saleDetails[index]?.discount > 0
                                    ? `${saleDetails[index].discount}${saleDetails[index].discountType === '%' ? '%' : 'Rs'}`
                                    : '-'}
                                </button>
                              )
                            ) : (
                              <div className={`text-center text-sm ${theme.textSecondary}`}>-</div>
                            )}
                          </div>
                        </td>
                        <td className={`py-2 px-3 text-right text-sm font-semibold ${theme.text}`}>
                          {lineTotal.toFixed(2)}
                        </td>
                        <td className="py-2 px-2 text-center">
                          <button
                            onClick={() => removeItem(index)}
                            className="text-red-400 hover:text-red-300 transition-colors p-1 rounded hover:bg-red-50 dark:hover:bg-red-900/20"
                            title="Remove item (Ctrl+Delete)"
                          >
                            <X size={14} />
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            )}
          </div>

          {/* Invoice Summary with Tax */}
          <div className={`${theme.tableHeader} border-t-2 ${theme.border} px-5 py-3`}>
            <div className="space-y-1.5">
              <div className="flex justify-between text-sm">
                <span className={theme.textSecondary}>Subtotal:</span>
                <span className={`${theme.text} font-medium`}>Rs. {subtotal.toFixed(2)}</span>
              </div>

              {/* Discount Type Toggle */}
              <div className="flex justify-between text-sm items-center">
                <div className="flex items-center space-x-2">
                  <span className={theme.textSecondary}>Discount Type:</span>
                  <div className="flex items-center space-x-1">
                    <button
                      onClick={() => setDiscountType('line')}
                      className={`px-2 py-0.5 text-xs rounded transition-colors ${discountType === 'line'
                        ? 'bg-blue-500 text-white'
                        : 'bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-gray-600'
                        }`}
                    >
                      Line Item
                    </button>
                    <button
                      onClick={() => setDiscountType('invoice')}
                      className={`px-2 py-0.5 text-xs rounded transition-colors ${discountType === 'invoice'
                        ? 'bg-blue-500 text-white'
                        : 'bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-gray-600'
                        }`}
                    >
                      Invoice
                    </button>
                  </div>
                </div>
                <span className="text-red-400 font-medium">- Rs. {totalDiscount.toFixed(2)}</span>
              </div>

              {/* Invoice Discount Input */}
              {discountType === 'invoice' && (
                <div className="flex items-center space-x-2 text-sm">
                  <span className={theme.textSecondary}>Invoice Discount:</span>
                  <div className="flex items-center space-x-1">
                    <input
                      type="number"
                      step="1"
                      min="0"
                      value={invoiceDiscount}
                      onChange={(e) => {
                        const val = e.target.value === '' ? 0 : parseFloat(e.target.value);
                        setValue('invoiceDiscount', isNaN(val) ? 0 : val);
                      }}
                      onFocus={(e) => e.target.select()}
                      className={`w-16 ${isDark ? 'bg-gray-700' : 'bg-gray-200'} text-xs ${theme.text} px-2 py-0.5 rounded outline-none`}
                    />
                    <select
                      {...register('invoiceDiscountType')}
                      className={`${isDark ? 'bg-gray-700' : 'bg-gray-200'} text-xs ${theme.text} px-2 py-0.5 rounded outline-none`}
                    >
                      <option value="Rs">Rs</option>
                      <option value="%">%</option>
                    </select>
                  </div>
                </div>
              )}

              {/* Tax Display */}
              {formTaxRate > 0 && (
                <div className="flex justify-between text-sm">
                  <span className={theme.textSecondary}>Tax ({formTaxRate}%):</span>
                  <span className={`${theme.text} font-medium`}>Rs. {taxAmount.toFixed(2)}</span>
                </div>
              )}

              <div className={`flex justify-between text-sm pb-2 border-b ${theme.border}`}>
                <span className={theme.textSecondary}>Total Discount:</span>
                <span className="text-red-400 font-medium">- Rs. {totalDiscount.toFixed(2)}</span>
              </div>

              <div className="flex justify-between pt-1.5">
                <span className={`text-base font-bold ${theme.text}`}>TOTAL:</span>
                <span className="text-xl font-bold text-blue-400">Rs. {total.toFixed(2)}</span>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className={`${theme.bgSecondary} px-5 py-3 border-t ${theme.border}`}>
            <div className="grid grid-cols-3 gap-2 mb-2">
              <button className={`flex items-center justify-center space-x-1.5 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} ${theme.text} px-3 py-2 rounded-lg transition-colors text-xs font-medium`}>
                <Pause size={14} />
                <span>Hold</span>
              </button>
              <button
                onClick={clearCart}
                className={`flex items-center justify-center space-x-1.5 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} ${theme.text} px-3 py-2 rounded-lg transition-colors text-xs font-medium`}
              >
                <Trash2 size={14} />
                <span>Clear (F1)</span>
              </button>
              <button
                onClick={handleEstimate}
                className={`flex items-center justify-center space-x-1.5 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} ${theme.text} px-3 py-2 rounded-lg transition-colors text-xs font-medium`}
              >
                <FileText size={14} />
                <span>Estimate</span>
              </button>
            </div>

            <div className="grid grid-cols-2 gap-2">
              <button
                onClick={() => handleCheckout('credit')}
                className="flex items-center justify-center space-x-2 bg-orange-600 hover:bg-orange-700 text-white px-4 py-2.5 rounded-lg transition-colors font-semibold text-sm"
              >
                <CreditCard size={16} />
                <span>Credit Sale (F6)</span>
              </button>
              <button
                onClick={() => handleCheckout('cash')}
                className="flex items-center justify-center space-x-2 bg-green-600 hover:bg-green-700 text-white px-4 py-2.5 rounded-lg transition-colors font-semibold text-sm shadow-lg shadow-green-900/50"
              >
                <DollarSign size={16} />
                <span>Cash Sale (F5)</span>
              </button>
            </div>
            {/* Shortcuts text removed as requested */}
          </div>
        </div>
      </div>

      {/* Payment Modal */}
      {showPaymentModal && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50">
          <div className={`${isDark ? 'bg-gray-800' : 'bg-white'} rounded-lg p-6 w-96 border ${theme.borderDark}`}>
            <h2 className={`text-xl font-bold mb-4 ${theme.text}`}>
              {saleType === 'cash' ? 'Cash Payment' : 'Credit Sale'}
            </h2>

            <div className="space-y-4">
              <div>
                <label className={`text-sm ${theme.textSecondary} block mb-1`}>Customer</label>
                <p className={`${theme.text} font-medium`}>{selectedCustomer?.name}</p>
              </div>

              <div className={`${isDark ? 'bg-gray-900' : 'bg-gray-100'} p-3 rounded`}>
                <div className="flex justify-between text-sm mb-1">
                  <span className={theme.textSecondary}>Total Amount:</span>
                  <span className={`${theme.text} font-bold text-lg`}>Rs. {total.toFixed(2)}</span>
                </div>
                {formTaxRate > 0 && (
                  <div className="flex justify-between text-xs">
                    <span className={theme.textSecondary}>Includes Tax ({formTaxRate}%):</span>
                    <span className={theme.textSecondary}>Rs. {taxAmount.toFixed(2)}</span>
                  </div>
                )}
                <div className="flex justify-between text-xs mt-1">
                  <span className={theme.textSecondary}>Sale Type:</span>
                  <span className={`font-medium ${saleType === 'cash' ? 'text-green-400' : 'text-orange-400'}`}>
                    {saleType.toUpperCase()}
                  </span>
                </div>
              </div>

              {saleType === 'cash' && (
                <>
                  <div>
                    <label className={`text-sm ${theme.textSecondary} block mb-1`}>Amount Received</label>
                    <input
                      type="number"
                      value={amountReceived}
                      onChange={(e) => setAmountReceived(e.target.value)}
                      placeholder="0.00"
                      className={`w-full ${isDark ? 'bg-gray-700' : 'bg-gray-100'} ${theme.text} px-4 py-2 rounded outline-none text-lg font-medium border ${theme.inputBorder}`}
                      autoFocus
                    />
                  </div>

                  {amountReceived && (
                    <div className="bg-green-900 bg-opacity-30 p-3 rounded border border-green-700">
                      <div className="flex justify-between">
                        <span className="text-green-400">Change to Return:</span>
                        <span className="text-green-300 font-bold text-lg">Rs. {changeReturn.toFixed(2)}</span>
                      </div>
                    </div>
                  )}
                </>
              )}

              {saleType === 'credit' && (
                <>
                  <div className="flex items-center space-x-2 mb-3">
                    <input
                      type="checkbox"
                      id="partialPayment"
                      checked={partialPayment}
                      onChange={(e) => setPartialPayment(e.target.checked)}
                      className="rounded border-gray-300"
                    />
                    <label htmlFor="partialPayment" className={`text-sm ${theme.text}`}>
                      Partial Payment
                    </label>
                  </div>

                  {partialPayment && (
                    <div>
                      <label className={`text-sm ${theme.textSecondary} block mb-1`}>Amount Received</label>
                      <input
                        type="number"
                        value={amountReceived}
                        onChange={(e) => setAmountReceived(e.target.value)}
                        placeholder="0.00"
                        className={`w-full ${isDark ? 'bg-gray-700' : 'bg-gray-100'} ${theme.text} px-4 py-2 rounded outline-none text-lg font-medium border ${theme.inputBorder}`}
                        autoFocus
                      />
                    </div>
                  )}

                  <div className="bg-blue-900 bg-opacity-30 p-3 rounded border border-blue-700">
                    <div className="flex justify-between text-sm mb-2">
                      <span className="text-blue-300">Total Due:</span>
                      <span className="text-blue-100 font-bold">Rs. {total.toFixed(2)}</span>
                    </div>
                    {partialPayment && amountReceived && (
                      <div className="flex justify-between text-sm">
                        <span className="text-blue-300">Amount Paid:</span>
                        <span className="text-green-300">Rs. {Number(amountReceived || 0).toFixed(2)}</span>
                      </div>
                    )}
                    <div className="flex justify-between text-sm mt-2 pt-2 border-t border-blue-600">
                      <span className="text-blue-300 font-semibold">Balance:</span>
                      <span className={`font-bold ${creditBalance > 0 ? 'text-orange-300' : 'text-green-300'}`}>
                        Rs. {creditBalance.toFixed(2)}
                      </span>
                    </div>
                  </div>
                </>
              )}
            </div>

            <div className="flex space-x-3 mt-6">
              <button
                onClick={() => setShowPaymentModal(false)}
                className={`flex-1 ${isDark ? 'bg-gray-700 hover:bg-gray-600' : 'bg-gray-200 hover:bg-gray-300'} ${theme.text} px-4 py-2 rounded-lg transition-colors font-medium`}
              >
                Cancel
              </button>
              <button
                onClick={completeSale}
                disabled={
                  isProcessing ||
                  (saleType === 'cash' && (!amountReceived || Number(amountReceived) < total)) ||
                  (saleType === 'credit' && partialPayment && (!amountReceived || Number(amountReceived) <= 0 || Number(amountReceived) > total))
                }
                className="flex-1 bg-green-600 hover:bg-green-700 disabled:bg-green-400 text-white px-4 py-2 rounded-lg transition-colors font-medium"
              >
                {isProcessing ? 'Processing...' : 'Complete Sale'}
              </button>
            </div>
          </div>
        </div>
      )}

      <CustomerModal
        isOpen={showAddCustomerModal}
        onClose={() => setShowAddCustomerModal(false)}
        onSuccess={(newCustomer) => {
          setSelectedCustomer(newCustomer);
          setValue('customerId', newCustomer.customerId);
        }}
      />
    </div>
  );
}