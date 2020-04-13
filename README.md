# Micky5991.Inventory

![Build status](https://github.com/Micky5991/Inventory/workflows/.NET%20Core/badge.svg?branch=master)

This library offers an framework to create virtual storage in games or other places where inventories are useful.
The concept based upon a virtual player-inventory that is similar to the [slot-based inventory of Minecraft](https://minecraft.gamepedia.com/Inventory).
It also features a framework for context actions on certain stacks of items.

## Installation

This library targets .NET Standard 2.0 and is available over NuGet.

```
PM> Install-Package Micky5991.Inventory
```

## Documentation

You can find further information about how to use this library in our [Documentation](docs/README.md).

## Concept

### Inventory

- The inventory is a collection of items that has an arbitrary finite capacity.
- It can contain unlimited item-instances but the total weight of all items cannot exceed the inventory capacity.
- If the inventory receives individual item instances, it tries to merge them into already inserted item instances based on specified strategies defined in items.
- Inventories are able to define which items are accepted.

**Examples**
- Inventory capacity of 10
    - 1 Apple (Weight 10)
- Inventory capacity of 10
    - 10 Apples (Weight 1)
- Inventory capacity of 10
    - 5 Apples (Weight 1)
    - 1 Peach (Weight 5)

### Item

- An item is a virtual element that contains actions and has an amount.
- Items are stackable by default if not defined otherwise in item meta.
- Characteristics of items are defined in item meta.
- The total weight of an item is based on the factors single-amount weight and the amount. `total-weight = single-weight * amount`
- The weight of an item-instance is not changable in runtime if not defined otherwise in item meta.
- Unstackable items cannot merge with the same kind and are limited to amount of 0 or 1.
- Items have display-names which are changable in runtime at any time.
- Each item can specify a strategy on how to merge with other item instances.
    - The default merge strategy has the following requirements
        - Both items are stackable.
        - Both items are from the same kind (handle).
        - Both items have equal single-weight.

### Item Registry

- Central collection of meta information about each kind of item.

### Item Meta

- Each meta entry has to be defined beforehand to the Item Registry.
- All implementations (child class of Item) can be used by multiple meta entries.
- Meta entries define certain characteristics of different kinds of items.
- Item flags can define if items are stackable or not or if the single-weight is changable or not.
- Item meta entries are unique by the meta-handle in the registry.
- Item meta can be used for basic informations without creating actual item instances.

## License

```
MIT License

Copyright (c) 2020 Francesco Paolocci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
