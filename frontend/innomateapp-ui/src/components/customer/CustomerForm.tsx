import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { CreateCustomerDto, Customer } from "@/types/customer.js";
import Button from "@/components/ui/Button.js";
import Input from "@/components/ui/Input.js";
import { useCustomerStore } from "@/store/usecustomerStore.js";

type Props = {
  customerId?: number | null;
  onClose: () => void;
};

export const CustomerForm: React.FC<Props> = ({ customerId = null, onClose }) => {
  const { register, handleSubmit, reset, setValue, formState: { errors, isSubmitting } } = useForm<CreateCustomerDto>({
    defaultValues: { name: "", email: "", phone: "", address: "" },
  });

  const fetchCustomer = useCustomerStore((s) => s.getCustomer);
  const createCustomer = useCustomerStore((s) => s.createCustomer);
  const updateCustomer = useCustomerStore((s) => s.updateCustomer);

  useEffect(() => {
    if (!customerId) {
      reset();
      return;
    }

    (async () => {
      const c = await fetchCustomer(customerId);
      if (c) {
        setValue("name", c.name ?? "");
        setValue("email", c.email ?? "");
        setValue("phone", c.phone ?? "");
        setValue("address", c.address ?? "");
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [customerId]);

  const onSubmit = async (data: CreateCustomerDto) => {
    if (customerId) {
      await updateCustomer(customerId, data);
    } else {
      await createCustomer(data);
    }
    onClose();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <Input label="Name" {...register("name", { required: "Name required" })} error={errors.name?.message} />
      <Input label="Email" {...register("email", { required: "Email required" })} error={errors.email?.message} />
      <Input label="Phone" {...register("phone")} />
      <Input label="Address" {...register("address")} />

      <div className="flex items-center justify-end gap-2 pt-4">
        <Button type="button" onClick={onClose}>Cancel</Button>
        <Button type="submit" disabled={isSubmitting}>{customerId ? "Update" : "Create"}</Button>
      </div>
    </form>
  );
};
