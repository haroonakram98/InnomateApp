"use client"

import { forwardRef } from "react"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/Select.js"

interface SelectFieldProps {
  value?: string | number
  onChange?: (value: string | number) => void
  options: Array<{ label: string; value: string | number }>
  disabled?: boolean
}

const SelectField = forwardRef<HTMLButtonElement, SelectFieldProps>(({ value, onChange, options, disabled }, ref) => {
  return (
    <Select value={String(value || "")} onValueChange={(val) => onChange?.(val)}>
      <SelectTrigger ref={ref} disabled={disabled}>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {options.map((option) => (
          <SelectItem key={option.value} value={String(option.value)}>
            {option.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
})

SelectField.displayName = "SelectField"

export default SelectField
