using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaSSDF.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<audit_log> audit_logs { get; set; }

    public virtual DbSet<cart> carts { get; set; }

    public virtual DbSet<cart_item> cart_items { get; set; }

    public virtual DbSet<category> categories { get; set; }

    public virtual DbSet<customer> customers { get; set; }

    public virtual DbSet<customer_address> customer_addresses { get; set; }

    public virtual DbSet<discount_type> discount_types { get; set; }

    public virtual DbSet<drug_form> drug_forms { get; set; }

    public virtual DbSet<inventory> inventories { get; set; }

    public virtual DbSet<inventory_movement> inventory_movements { get; set; }

    public virtual DbSet<inventory_movement_type> inventory_movement_types { get; set; }

    public virtual DbSet<laboratory> laboratories { get; set; }

    public virtual DbSet<measurement_unit> measurement_units { get; set; }

    public virtual DbSet<order> orders { get; set; }

    public virtual DbSet<order_item> order_items { get; set; }

    public virtual DbSet<order_payment> order_payments { get; set; }

    public virtual DbSet<order_status> order_statuses { get; set; }

    public virtual DbSet<order_status_history> order_status_histories { get; set; }

    public virtual DbSet<payment_method> payment_methods { get; set; }

    public virtual DbSet<prescription_upload> prescription_uploads { get; set; }

    public virtual DbSet<product> products { get; set; }

    public virtual DbSet<product_batch> product_batches { get; set; }

    public virtual DbSet<product_image> product_images { get; set; }

    public virtual DbSet<product_variant> product_variants { get; set; }

    public virtual DbSet<promotion> promotions { get; set; }

    public virtual DbSet<promotion_code> promotion_codes { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user1> users1 { get; set; }

    public virtual DbSet<user_role> user_roles { get; set; }

    public virtual DbSet<v_available_stock> v_available_stocks { get; set; }

    public virtual DbSet<v_customer_order_summary> v_customer_order_summaries { get; set; }

    public virtual DbSet<v_expiring_batch> v_expiring_batches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<audit_log>(entity =>
        {
            entity.HasKey(e => e.audit_log_id).HasName("audit_logs_pkey");

            entity.ToTable(tb => tb.HasComment("Log de auditoría para acciones sensibles (cambios de precio, roles, stock). Inmutable. Usar BIGSERIAL por volumen de escritura. Considerar particionamiento por created_at cuando supere 5M filas."));

            entity.HasIndex(e => e.created_at, "ix_audit_created").IsDescending();

            entity.HasIndex(e => new { e.table_name, e.record_id }, "ix_audit_table_record");

            entity.HasIndex(e => e.user_id, "ix_audit_user").HasFilter("(user_id IS NOT NULL)");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.new_values)
                .HasComment("Estado nuevo del registro en JSONB. NULL en DELETE.")
                .HasColumnType("jsonb");
            entity.Property(e => e.old_values)
                .HasComment("Estado anterior del registro en JSONB. NULL en INSERT.")
                .HasColumnType("jsonb");
            entity.Property(e => e.record_id).HasComment("UUID o ID del registro afectado como TEXT para compatibilidad con cualquier tipo de PK.");

            entity.HasOne(d => d.customer).WithMany(p => p.audit_logs)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audit_logs_customer_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.audit_logs)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audit_logs_user_id_fkey");
        });

        modelBuilder.Entity<cart>(entity =>
        {
            entity.HasKey(e => e.cart_id).HasName("carts_pkey");

            entity.ToTable(tb => tb.HasComment("Carritos de compra persistentes. customer_id NULL = carrito anónimo identificado por session_id. Al hacer login, el carrito anónimo se asocia al cliente."));

            entity.HasIndex(e => new { e.customer_id, e.is_active }, "ix_carts_customer_active").HasFilter("((is_active = true) AND (customer_id IS NOT NULL))");

            entity.HasIndex(e => new { e.session_id, e.is_active }, "ix_carts_session_active").HasFilter("((is_active = true) AND (session_id IS NOT NULL))");

            entity.Property(e => e.cart_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.expires_at).HasComment("Fecha de expiración del carrito. Limpiar carritos expirados con un job periódico.");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.customer).WithMany(p => p.carts)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("carts_customer_id_fkey");
        });

        modelBuilder.Entity<cart_item>(entity =>
        {
            entity.HasKey(e => e.cart_item_id).HasName("cart_items_pkey");

            entity.ToTable(tb => tb.HasComment("Líneas del carrito. Una por variante. El precio se recalcula al hacer checkout; unit_price_snapshot es solo referencia."));

            entity.HasIndex(e => e.cart_id, "ix_cart_items_cart");

            entity.HasIndex(e => new { e.cart_id, e.product_variant_id }, "uq_cart_variant").IsUnique();

            entity.Property(e => e.cart_item_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.added_at).HasDefaultValueSql("now()");
            entity.Property(e => e.quantity).HasDefaultValue(1);
            entity.Property(e => e.unit_price_snapshot)
                .HasPrecision(18, 2)
                .HasComment("Precio al momento de agregar al carrito. El precio real de venta se toma de product_variants.price al confirmar.");

            entity.HasOne(d => d.cart).WithMany(p => p.cart_items)
                .HasForeignKey(d => d.cart_id)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.product_variant).WithMany(p => p.cart_items)
                .HasForeignKey(d => d.product_variant_id)
                .HasConstraintName("cart_items_product_variant_id_fkey");
        });

        modelBuilder.Entity<category>(entity =>
        {
            entity.HasKey(e => e.category_id).HasName("categories_pkey");

            entity.ToTable(tb => tb.HasComment("Clasificación jerárquica de medicamentos. Soporta subcategorías via auto-referencia."));

            entity.HasIndex(e => e.slug, "categories_slug_key").IsUnique();

            entity.Property(e => e.category_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.parent_category_id).HasComment("NULL = categoría raíz. Permite un nivel de subcategorías.");
            entity.Property(e => e.slug).HasComment("Identificador URL-friendly único. Ej: analgesicos, antibioticos.");
            entity.Property(e => e.sort_order).HasDefaultValue(0);

            entity.HasOne(d => d.parent_category).WithMany(p => p.Inverseparent_category)
                .HasForeignKey(d => d.parent_category_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("categories_parent_category_id_fkey");
        });

        modelBuilder.Entity<customer>(entity =>
        {
            entity.HasKey(e => e.customer_id).HasName("customers_pkey");

            entity.ToTable(tb => tb.HasComment("Compradores del eCommerce. Separados de users (personal interno) por tener flujos, permisos y datos distintos."));

            entity.HasIndex(e => e.auth_user_id, "customers_auth_user_id_key").IsUnique();

            entity.HasIndex(e => e.email, "customers_email_key").IsUnique();

            entity.HasIndex(e => e.email, "ix_customers_email")
                .IsUnique()
                .HasFilter("(deleted_at IS NULL)");

            entity.Property(e => e.customer_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.deleted_at).HasComment("Soft delete. Preserva historial de pedidos del cliente.");
            entity.Property(e => e.document_number).HasComment("DNI, RUC, pasaporte u otro documento de identidad.");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_email_verified).HasDefaultValue(false);

            entity.HasOne(d => d.auth_user).WithOne(p => p.customer)
                .HasForeignKey<customer>(d => d.auth_user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("customers_auth_user_id_fkey");
        });

        modelBuilder.Entity<customer_address>(entity =>
        {
            entity.HasKey(e => e.address_id).HasName("customer_addresses_pkey");

            entity.ToTable(tb => tb.HasComment("Direcciones de envío guardadas por el cliente. Un cliente puede tener varias; is_default indica la preferida."));

            entity.HasIndex(e => e.customer_id, "ix_customer_addresses_customer").HasFilter("(is_active = true)");

            entity.Property(e => e.address_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.country).HasDefaultValueSql("'Peru'::text");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_default)
                .HasDefaultValue(false)
                .HasComment("Solo debe haber una dirección por defecto por cliente. Validar en aplicación.");
            entity.Property(e => e.label).HasComment("Etiqueta descriptiva elegida por el cliente. Ej: Casa, Trabajo, Farmacia.");

            entity.HasOne(d => d.customer).WithMany(p => p.customer_addresses)
                .HasForeignKey(d => d.customer_id)
                .HasConstraintName("customer_addresses_customer_id_fkey");
        });

        modelBuilder.Entity<discount_type>(entity =>
        {
            entity.HasKey(e => e.discount_type_id).HasName("discount_types_pkey");

            entity.ToTable(tb => tb.HasComment("Determina cómo interpretar el campo discount_value en promotions. Seed: Percentage, FixedAmount."));

            entity.HasIndex(e => e.name, "discount_types_name_key").IsUnique();

            entity.Property(e => e.discount_type_id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<drug_form>(entity =>
        {
            entity.HasKey(e => e.drug_form_id).HasName("drug_forms_pkey");

            entity.ToTable(tb => tb.HasComment("Catálogo de formas farmacéuticas. Seed: Tableta, Cápsula, Jarabe, Suspensión, Inyectable, Crema, Gel, Parche, Gotas, Spray, Supositorio, Polvo."));

            entity.HasIndex(e => e.name, "drug_forms_name_key").IsUnique();

            entity.Property(e => e.drug_form_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<inventory>(entity =>
        {
            entity.HasKey(e => e.inventory_id).HasName("inventory_pkey");

            entity.ToTable("inventory", tb => tb.HasComment("Estado actual del stock por variante. Permite consulta O(1) de disponibilidad sin sumar movimientos históricos."));

            entity.HasIndex(e => e.product_variant_id, "inventory_product_variant_id_key").IsUnique();

            entity.HasIndex(e => new { e.quantity_on_hand, e.min_stock_level }, "ix_inventory_low_stock").HasFilter("(quantity_on_hand <= min_stock_level)");

            entity.HasIndex(e => e.product_variant_id, "ix_inventory_variant");

            entity.Property(e => e.inventory_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.last_updated_at).HasDefaultValueSql("now()");
            entity.Property(e => e.min_stock_level)
                .HasDefaultValue(5)
                .HasComment("Umbral de alerta de bajo stock. Genera notificación cuando quantity_on_hand <= min_stock_level.");
            entity.Property(e => e.quantity_on_hand)
                .HasDefaultValue(0)
                .HasComment("Stock físico real en almacén.");
            entity.Property(e => e.reserved_quantity)
                .HasDefaultValue(0)
                .HasComment("Stock reservado en pedidos confirmados pero no despachados aún.");

            entity.HasOne(d => d.product_variant).WithOne(p => p.inventory)
                .HasForeignKey<inventory>(d => d.product_variant_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("inventory_product_variant_id_fkey");
        });

        modelBuilder.Entity<inventory_movement>(entity =>
        {
            entity.HasKey(e => e.movement_id).HasName("inventory_movements_pkey");

            entity.ToTable(tb => tb.HasComment("Historial auditado de todos los cambios de stock. Inmutable. Permite reconciliar discrepancias y generar reportes de entradas/salidas."));

            entity.HasIndex(e => e.batch_id, "ix_inv_movements_batch").HasFilter("(batch_id IS NOT NULL)");

            entity.HasIndex(e => new { e.reference_type, e.reference_id }, "ix_inv_movements_reference").HasFilter("(reference_id IS NOT NULL)");

            entity.HasIndex(e => new { e.product_variant_id, e.created_at }, "ix_inv_movements_variant_date").IsDescending(false, true);

            entity.Property(e => e.movement_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.quantity).HasComment("Cantidad siempre positiva. La dirección (entrada/salida) la determina inventory_movement_types.direction.");
            entity.Property(e => e.reference_id).HasComment("UUID de la entidad originadora (order_id, batch_id, etc.).");
            entity.Property(e => e.reference_type).HasComment("Tipo de la entidad que originó el movimiento.");

            entity.HasOne(d => d.batch).WithMany(p => p.inventory_movements)
                .HasForeignKey(d => d.batch_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("inventory_movements_batch_id_fkey");

            entity.HasOne(d => d.movement_type).WithMany(p => p.inventory_movements)
                .HasForeignKey(d => d.movement_type_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("inventory_movements_movement_type_id_fkey");

            entity.HasOne(d => d.product_variant).WithMany(p => p.inventory_movements)
                .HasForeignKey(d => d.product_variant_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("inventory_movements_product_variant_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.inventory_movements)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("inventory_movements_user_id_fkey");
        });

        modelBuilder.Entity<inventory_movement_type>(entity =>
        {
            entity.HasKey(e => e.movement_type_id).HasName("inventory_movement_types_pkey");

            entity.ToTable(tb => tb.HasComment("Clasifica cada movimiento de inventario y su dirección. La cantidad en inventory_movements siempre es positiva."));

            entity.HasIndex(e => e.name, "inventory_movement_types_name_key").IsUnique();

            entity.Property(e => e.movement_type_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.direction)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("IN = entrada de stock. OUT = salida de stock.");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<laboratory>(entity =>
        {
            entity.HasKey(e => e.laboratory_id).HasName("laboratories_pkey");

            entity.ToTable(tb => tb.HasComment("Fabricantes de medicamentos. Un laboratorio puede fabricar múltiples productos. Separado para evitar redundancia y permitir filtrado por marca."));

            entity.Property(e => e.laboratory_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<measurement_unit>(entity =>
        {
            entity.HasKey(e => e.unit_id).HasName("measurement_units_pkey");

            entity.ToTable(tb => tb.HasComment("Unidades de concentración y volumen. Seed: mg, ml, g, mcg, UI, %."));

            entity.HasIndex(e => e.name, "measurement_units_name_key").IsUnique();

            entity.Property(e => e.unit_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<order>(entity =>
        {
            entity.HasKey(e => e.order_id).HasName("orders_pkey");

            entity.ToTable(tb => tb.HasComment("Cabecera del pedido. Documento contable inmutable. Los montos son snapshot al momento de confirmar y NUNCA se recalculan."));

            entity.HasIndex(e => new { e.customer_id, e.created_at }, "ix_orders_customer_created").IsDescending(false, true);

            entity.HasIndex(e => e.order_number, "ix_orders_number").IsUnique();

            entity.HasIndex(e => new { e.order_status_id, e.created_at }, "ix_orders_status_created").IsDescending(false, true);

            entity.HasIndex(e => e.order_number, "orders_order_number_key").IsUnique();

            entity.Property(e => e.order_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.discount_amount).HasPrecision(18, 2);
            entity.Property(e => e.order_number).HasComment("Identificador legible para el cliente. Ej: ORD-2024-000001. Generado por la aplicación con una secuencia.");
            entity.Property(e => e.shipping_cost).HasPrecision(18, 2);
            entity.Property(e => e.subtotal).HasPrecision(18, 2);
            entity.Property(e => e.tax_amount).HasPrecision(18, 2);
            entity.Property(e => e.total)
                .HasPrecision(18, 2)
                .HasComment("subtotal + tax_amount + shipping_cost - discount_amount. Calculado y fijado al confirmar el pedido.");

            entity.HasOne(d => d.customer).WithMany(p => p.orders)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orders_customer_id_fkey");

            entity.HasOne(d => d.order_status).WithMany(p => p.orders)
                .HasForeignKey(d => d.order_status_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orders_order_status_id_fkey");

            entity.HasOne(d => d.promotion_code).WithMany(p => p.orders)
                .HasForeignKey(d => d.promotion_code_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("orders_promotion_code_id_fkey");

            entity.HasOne(d => d.shipping_address).WithMany(p => p.orders)
                .HasForeignKey(d => d.shipping_address_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("orders_shipping_address_id_fkey");
        });

        modelBuilder.Entity<order_item>(entity =>
        {
            entity.HasKey(e => e.order_item_id).HasName("order_items_pkey");

            entity.ToTable(tb => tb.HasComment("Líneas del pedido. Inmutables una vez creadas. Los snapshots garantizan integridad histórica aunque el producto cambie después."));

            entity.HasIndex(e => e.order_id, "ix_order_items_order");

            entity.HasIndex(e => e.product_variant_id, "ix_order_items_variant");

            entity.Property(e => e.order_item_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.batch_id).HasComment("Lote del que se extrajo el medicamento. Asignado al despachar según lógica FEFO.");
            entity.Property(e => e.discount_amount).HasPrecision(18, 2);
            entity.Property(e => e.product_name_snapshot).HasComment("Nombre del producto al momento de la compra.");
            entity.Property(e => e.sku_snapshot).HasComment("SKU al momento de la compra para trazabilidad contable.");
            entity.Property(e => e.subtotal).HasPrecision(18, 2);
            entity.Property(e => e.unit_price)
                .HasPrecision(18, 2)
                .HasComment("Precio unitario al momento de la compra. Snapshot inmutable.");
            entity.Property(e => e.variant_desc_snapshot).HasComment("Descripción de la variante al momento de la compra. Ej: Tableta 500mg x30.");

            entity.HasOne(d => d.batch).WithMany(p => p.order_items)
                .HasForeignKey(d => d.batch_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_items_batch_id_fkey");

            entity.HasOne(d => d.order).WithMany(p => p.order_items)
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.prescription).WithMany(p => p.order_items)
                .HasForeignKey(d => d.prescription_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_items_prescription_id_fkey");

            entity.HasOne(d => d.product_variant).WithMany(p => p.order_items)
                .HasForeignKey(d => d.product_variant_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_items_product_variant_id_fkey");
        });

        modelBuilder.Entity<order_payment>(entity =>
        {
            entity.HasKey(e => e.payment_id).HasName("order_payments_pkey");

            entity.ToTable(tb => tb.HasComment("Registros de pago por pedido. Permite múltiples intentos y pagos parciales. payment_status maneja el ciclo de vida del pago."));

            entity.HasIndex(e => e.order_id, "ix_order_payments_order");

            entity.Property(e => e.payment_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.amount).HasPrecision(18, 2);
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.payment_status).HasDefaultValueSql("'Pending'::text");
            entity.Property(e => e.transaction_reference).HasComment("Referencia externa del pago. Ej: código de operación Yape, número de transferencia bancaria.");

            entity.HasOne(d => d.order).WithMany(p => p.order_payments)
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_payments_order_id_fkey");

            entity.HasOne(d => d.payment_method).WithMany(p => p.order_payments)
                .HasForeignKey(d => d.payment_method_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_payments_payment_method_id_fkey");
        });

        modelBuilder.Entity<order_status>(entity =>
        {
            entity.HasKey(e => e.order_status_id).HasName("order_statuses_pkey");

            entity.ToTable(tb => tb.HasComment("Estados del ciclo de vida de un pedido. El historial de cambios se guarda en order_status_history."));

            entity.HasIndex(e => e.name, "order_statuses_name_key").IsUnique();

            entity.Property(e => e.order_status_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.sort_order).HasDefaultValue(0);
        });

        modelBuilder.Entity<order_status_history>(entity =>
        {
            entity.HasKey(e => e.history_id).HasName("order_status_history_pkey");

            entity.ToTable("order_status_history", tb => tb.HasComment("Trazabilidad completa de cambios de estado del pedido. Inmutable. orders.order_status_id siempre refleja el estado actual."));

            entity.HasIndex(e => new { e.order_id, e.created_at }, "ix_order_status_history_order").IsDescending(false, true);

            entity.Property(e => e.history_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.changed_by_user_id).HasComment("Usuario que realizó el cambio. NULL si fue un proceso automático del sistema.");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.changed_by_user).WithMany(p => p.order_status_histories)
                .HasForeignKey(d => d.changed_by_user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_status_history_changed_by_user_id_fkey");

            entity.HasOne(d => d.order).WithMany(p => p.order_status_histories)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("order_status_history_order_id_fkey");

            entity.HasOne(d => d.order_status).WithMany(p => p.order_status_histories)
                .HasForeignKey(d => d.order_status_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_status_history_order_status_id_fkey");
        });

        modelBuilder.Entity<payment_method>(entity =>
        {
            entity.HasKey(e => e.payment_method_id).HasName("payment_methods_pkey");

            entity.ToTable(tb => tb.HasComment("Métodos de pago habilitados. is_online diferencia pagos digitales de presenciales."));

            entity.HasIndex(e => e.name, "payment_methods_name_key").IsUnique();

            entity.Property(e => e.payment_method_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_online).HasDefaultValue(false);
        });

        modelBuilder.Entity<prescription_upload>(entity =>
        {
            entity.HasKey(e => e.prescription_id).HasName("prescription_uploads_pkey");

            entity.ToTable(tb => tb.HasComment("Recetas médicas subidas por clientes. Obligatoria para productos con requires_prescription = TRUE. El pedido no avanza a Processing sin verificación."));

            entity.HasIndex(e => e.customer_id, "ix_prescriptions_customer");

            entity.HasIndex(e => new { e.is_verified, e.created_at }, "ix_prescriptions_unverified").HasFilter("(is_verified = false)");

            entity.Property(e => e.prescription_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.image_url).HasComment("URL del archivo en Supabase Storage. El bucket debe ser privado con acceso controlado.");
            entity.Property(e => e.is_verified)
                .HasDefaultValue(false)
                .HasComment("Un farmacéutico (user) debe verificar la receta antes de despachar.");
            entity.Property(e => e.rejection_reason).HasComment("Motivo de rechazo de la receta. Visible para el cliente.");

            entity.HasOne(d => d.customer).WithMany(p => p.prescription_uploads)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("prescription_uploads_customer_id_fkey");

            entity.HasOne(d => d.order).WithMany(p => p.prescription_uploads)
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("prescription_uploads_order_id_fkey");

            entity.HasOne(d => d.verified_by_user).WithMany(p => p.prescription_uploads)
                .HasForeignKey(d => d.verified_by_user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("prescription_uploads_verified_by_user_id_fkey");
        });

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.product_id).HasName("products_pkey");

            entity.ToTable(tb => tb.HasComment("Medicamento como concepto abstracto. No se vende directamente; las variantes (product_variants) son las unidades vendibles."));

            entity.HasIndex(e => new { e.category_id, e.is_active }, "ix_products_category_active").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.generic_name, "ix_products_generic_name_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => new { e.laboratory_id, e.is_active }, "ix_products_laboratory_active").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.name, "ix_products_name_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.slug, "ix_products_slug").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.slug, "products_slug_key").IsUnique();

            entity.Property(e => e.product_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.deleted_at).HasComment("Soft delete. NULL = activo. Filtrar siempre con deleted_at IS NULL.");
            entity.Property(e => e.generic_name).HasComment("Denominación Común Internacional (DCI).");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.is_controlled)
                .HasDefaultValue(false)
                .HasComment("Sustancia controlada. Requiere restricciones adicionales de venta.");
            entity.Property(e => e.requires_prescription)
                .HasDefaultValue(false)
                .HasComment("Si TRUE, el pedido requiere receta médica verificada antes de despachar.");
            entity.Property(e => e.slug).HasComment("URL amigable única. Ej: paracetamol-genfar.");
            entity.Property(e => e.tags).HasComment("Palabras clave separadas por coma para búsqueda interna.");

            entity.HasOne(d => d.category).WithMany(p => p.products)
                .HasForeignKey(d => d.category_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_category_id_fkey");

            entity.HasOne(d => d.laboratory).WithMany(p => p.products)
                .HasForeignKey(d => d.laboratory_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("products_laboratory_id_fkey");
        });

        modelBuilder.Entity<product_batch>(entity =>
        {
            entity.HasKey(e => e.batch_id).HasName("product_batches_pkey");

            entity.ToTable(tb => tb.HasComment("Lotes de medicamentos con trazabilidad de vencimiento. Obligatorio para regulación farmacéutica. Lógica FEFO aplicada al despachar."));

            entity.HasIndex(e => e.expiration_date, "ix_batches_expiration").HasFilter("(is_active = true)");

            entity.HasIndex(e => e.expiration_date, "ix_batches_expiring_soon").HasFilter("((is_active = true) AND (current_quantity > 0))");

            entity.HasIndex(e => e.product_variant_id, "ix_batches_variant");

            entity.HasIndex(e => new { e.product_variant_id, e.batch_number }, "uq_batch_variant").IsUnique();

            entity.Property(e => e.batch_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.batch_number).HasComment("Número de lote impreso en el empaque del fabricante.");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.expiration_date).HasComment("Fecha de vencimiento. Nunca vender si expiration_date < NOW().");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.laboratory).WithMany(p => p.product_batches)
                .HasForeignKey(d => d.laboratory_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("product_batches_laboratory_id_fkey");

            entity.HasOne(d => d.product_variant).WithMany(p => p.product_batches)
                .HasForeignKey(d => d.product_variant_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_batches_product_variant_id_fkey");
        });

        modelBuilder.Entity<product_image>(entity =>
        {
            entity.HasKey(e => e.product_image_id).HasName("product_images_pkey");

            entity.ToTable(tb => tb.HasComment("Imágenes a nivel de producto genérico o variante específica. Solo se guarda la URL; el archivo vive en Supabase Storage o CDN externo."));

            entity.HasIndex(e => new { e.product_id, e.is_main }, "ix_product_images_product_main").HasFilter("(product_id IS NOT NULL)");

            entity.HasIndex(e => new { e.product_variant_id, e.is_main }, "ix_product_images_variant_main").HasFilter("(product_variant_id IS NOT NULL)");

            entity.Property(e => e.product_image_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.image_url).HasComment("URL pública del archivo. Puede ser Supabase Storage URL.");
            entity.Property(e => e.is_main)
                .HasDefaultValue(false)
                .HasComment("Imagen principal que se muestra en el listado del catálogo.");
            entity.Property(e => e.sort_order).HasDefaultValue(0);

            entity.HasOne(d => d.product).WithMany(p => p.product_images)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_images_product_id_fkey");

            entity.HasOne(d => d.product_variant).WithMany(p => p.product_images)
                .HasForeignKey(d => d.product_variant_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("product_images_product_variant_id_fkey");
        });

        modelBuilder.Entity<product_variant>(entity =>
        {
            entity.HasKey(e => e.product_variant_id).HasName("product_variants_pkey");

            entity.ToTable(tb => tb.HasComment("Presentación específica y vendible de un medicamento. Ej: Paracetamol 500mg x30 tabletas. Todo lo operativo (precio, stock, SKU) vive aquí."));

            entity.HasIndex(e => e.barcode, "ix_variants_barcode").HasFilter("(barcode IS NOT NULL)");

            entity.HasIndex(e => new { e.product_id, e.is_active }, "ix_variants_product_active").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.sku, "ix_variants_sku");

            entity.HasIndex(e => e.barcode, "product_variants_barcode_key").IsUnique();

            entity.HasIndex(e => e.sku, "product_variants_sku_key").IsUnique();

            entity.Property(e => e.product_variant_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.compare_at_price)
                .HasPrecision(18, 2)
                .HasComment("Precio original antes del descuento. Se muestra tachado en el eCommerce.");
            entity.Property(e => e.concentration)
                .HasPrecision(10, 3)
                .HasComment("Valor numérico de la concentración. La unidad se define en unit_id.");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.deleted_at).HasComment("Soft delete. Las variantes no se borran para preservar historial de pedidos.");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.package_size)
                .HasDefaultValue(1)
                .HasComment("Unidades por empaque. Ej: 30 (tabletas por caja).");
            entity.Property(e => e.price).HasPrecision(18, 2);
            entity.Property(e => e.sort_order).HasDefaultValue(0);

            entity.HasOne(d => d.drug_form).WithMany(p => p.product_variants)
                .HasForeignKey(d => d.drug_form_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_variants_drug_form_id_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.product_variants)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_variants_product_id_fkey");

            entity.HasOne(d => d.unit).WithMany(p => p.product_variants)
                .HasForeignKey(d => d.unit_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_variants_unit_id_fkey");

            entity.HasMany(d => d.promotions).WithMany(p => p.product_variants)
                .UsingEntity<Dictionary<string, object>>(
                    "product_promotion",
                    r => r.HasOne<promotion>().WithMany()
                        .HasForeignKey("promotion_id")
                        .HasConstraintName("product_promotions_promotion_id_fkey"),
                    l => l.HasOne<product_variant>().WithMany()
                        .HasForeignKey("product_variant_id")
                        .HasConstraintName("product_promotions_product_variant_id_fkey"),
                    j =>
                    {
                        j.HasKey("product_variant_id", "promotion_id").HasName("product_promotions_pkey");
                        j.ToTable("product_promotions", tb => tb.HasComment("Tabla puente N:M. Indica qué variantes participan en cada promoción. Solo aplica cuando promotions.applies_to_all = FALSE."));
                        j.HasIndex(new[] { "promotion_id" }, "ix_product_promotions_promo");
                        j.HasIndex(new[] { "product_variant_id" }, "ix_product_promotions_variant");
                    });
        });

        modelBuilder.Entity<promotion>(entity =>
        {
            entity.HasKey(e => e.promotion_id).HasName("promotions_pkey");

            entity.ToTable(tb => tb.HasComment("Descuentos y promociones con reglas de vigencia, límite de uso y monto mínimo de pedido."));

            entity.HasIndex(e => new { e.is_active, e.start_date, e.end_date }, "ix_promotions_active_dates").HasFilter("(is_active = true)");

            entity.Property(e => e.promotion_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.applies_to_all)
                .HasDefaultValue(false)
                .HasComment("TRUE = aplica a todo el catálogo. FALSE = solo a variantes listadas en product_promotions.");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.current_uses).HasDefaultValue(0);
            entity.Property(e => e.discount_value)
                .HasPrecision(10, 4)
                .HasComment("Valor del descuento. Interpretar según discount_type_id: porcentaje (10 = 10%) o monto fijo (10 = S/.10).");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.max_discount_amount)
                .HasPrecision(18, 2)
                .HasComment("Tope máximo de descuento en monto fijo. Útil para descuentos porcentuales con límite.");
            entity.Property(e => e.min_order_amount).HasPrecision(18, 2);

            entity.HasOne(d => d.discount_type).WithMany(p => p.promotions)
                .HasForeignKey(d => d.discount_type_id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("promotions_discount_type_id_fkey");
        });

        modelBuilder.Entity<promotion_code>(entity =>
        {
            entity.HasKey(e => e.code_id).HasName("promotion_codes_pkey");

            entity.ToTable(tb => tb.HasComment("Códigos de cupón asociados a una promoción. Ej: VERANO10. Tienen control de usos propio además del límite de la promoción."));

            entity.HasIndex(e => e.code, "promotion_codes_code_key").IsUnique();

            entity.Property(e => e.code_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.code).HasComment("Código que ingresa el cliente al hacer checkout. Único en todo el sistema.");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.current_uses).HasDefaultValue(0);
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.promotion).WithMany(p => p.promotion_codes)
                .HasForeignKey(d => d.promotion_id)
                .HasConstraintName("promotion_codes_promotion_id_fkey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.role_id).HasName("roles_pkey");

            entity.ToTable(tb => tb.HasComment("Roles para control de acceso del personal interno. Un usuario puede tener múltiples roles via user_roles."));

            entity.HasIndex(e => e.name, "roles_name_key").IsUnique();

            entity.Property(e => e.role_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.ToTable("users", "auth", tb => tb.HasComment("Auth: Stores user login data within a secure schema."));

            entity.HasIndex(e => e.confirmation_token, "confirmation_token_idx")
                .IsUnique()
                .HasFilter("((confirmation_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.email_change_token_current, "email_change_token_current_idx")
                .IsUnique()
                .HasFilter("((email_change_token_current)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.email_change_token_new, "email_change_token_new_idx")
                .IsUnique()
                .HasFilter("((email_change_token_new)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.created_at, "idx_users_created_at_desc").IsDescending();

            entity.HasIndex(e => e.email, "idx_users_email");

            entity.HasIndex(e => e.last_sign_in_at, "idx_users_last_sign_in_at_desc").IsDescending();

            entity.HasIndex(e => e.reauthentication_token, "reauthentication_token_idx")
                .IsUnique()
                .HasFilter("((reauthentication_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.recovery_token, "recovery_token_idx")
                .IsUnique()
                .HasFilter("((recovery_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.email, "users_email_partial_key")
                .IsUnique()
                .HasFilter("(is_sso_user = false)");

            entity.HasIndex(e => e.instance_id, "users_instance_id_idx");

            entity.HasIndex(e => e.is_anonymous, "users_is_anonymous_idx");

            entity.HasIndex(e => e.phone, "users_phone_key").IsUnique();

            entity.Property(e => e.id).ValueGeneratedNever();
            entity.Property(e => e.aud).HasMaxLength(255);
            entity.Property(e => e.confirmation_token).HasMaxLength(255);
            entity.Property(e => e.confirmed_at).HasComputedColumnSql("LEAST(email_confirmed_at, phone_confirmed_at)", true);
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.email_change).HasMaxLength(255);
            entity.Property(e => e.email_change_confirm_status).HasDefaultValue((short)0);
            entity.Property(e => e.email_change_token_current)
                .HasMaxLength(255)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.email_change_token_new).HasMaxLength(255);
            entity.Property(e => e.encrypted_password).HasMaxLength(255);
            entity.Property(e => e.is_anonymous).HasDefaultValue(false);
            entity.Property(e => e.is_sso_user)
                .HasDefaultValue(false)
                .HasComment("Auth: Set this column to true when the account comes from SSO. These accounts can have duplicate emails.");
            entity.Property(e => e.phone).HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.phone_change).HasDefaultValueSql("''::character varying");
            entity.Property(e => e.phone_change_token)
                .HasMaxLength(255)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.raw_app_meta_data).HasColumnType("jsonb");
            entity.Property(e => e.raw_user_meta_data).HasColumnType("jsonb");
            entity.Property(e => e.reauthentication_token)
                .HasMaxLength(255)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.recovery_token).HasMaxLength(255);
            entity.Property(e => e.role).HasMaxLength(255);
        });

        modelBuilder.Entity<user1>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Personal interno: administradores, farmacéuticos, vendedores. Separado de customers. Accede al backoffice, no al eCommerce público."));

            entity.HasIndex(e => e.email, "ix_users_email")
                .IsUnique()
                .HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.auth_user_id, "users_auth_user_id_key").IsUnique();

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.Property(e => e.user_id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.auth_user).WithOne(p => p.user1)
                .HasForeignKey<user1>(d => d.auth_user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_auth_user_id_fkey");
        });

        modelBuilder.Entity<user_role>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.role_id }).HasName("user_roles_pkey");

            entity.ToTable(tb => tb.HasComment("Tabla puente N:M entre users y roles. Un usuario puede tener múltiples roles. Registra quién asignó el rol."));

            entity.HasIndex(e => e.role_id, "ix_user_roles_role");

            entity.HasIndex(e => e.user_id, "ix_user_roles_user");

            entity.Property(e => e.assigned_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.assigned_by_user).WithMany(p => p.user_roleassigned_by_users)
                .HasForeignKey(d => d.assigned_by_user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_roles_assigned_by_user_id_fkey");

            entity.HasOne(d => d.role).WithMany(p => p.user_roles)
                .HasForeignKey(d => d.role_id)
                .HasConstraintName("user_roles_role_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.user_roleusers)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("user_roles_user_id_fkey");
        });

        modelBuilder.Entity<v_available_stock>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_available_stock");

            entity.Property(e => e.concentration).HasPrecision(10, 3);
        });

        modelBuilder.Entity<v_customer_order_summary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_customer_order_summary");
        });

        modelBuilder.Entity<v_expiring_batch>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_expiring_batches");
        });
        modelBuilder.HasSequence("order_number_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
