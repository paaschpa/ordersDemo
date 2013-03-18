Ext.define('Order', {
    extend: 'Ext.data.Model',
    fields: [
        'id',
        'customerName',
        'itemName',
        'quantity'
    ]
});

Ext.define('Fulfillment', {
    extend: 'Ext.data.Model',
    fields: [
        'id',
        'itemName',
        'quantity',
        'fulfiller',
        'status'
    ]
});
