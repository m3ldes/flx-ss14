# SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 Aiden <aiden@djkraz.com>
# SPDX-FileCopyrightText: 2025 SX-7 <92227810+SX-7@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

reagent-effect-condition-guidebook-stamina-damage-threshold =
    { $max ->
        [2147483648] у цели как минимум {NATURALFIXED($min, 2)} повреждений выносливости
        *[other] { $min ->
                    [0] у цели не более {NATURALFIXED($max, 2)} повреждений выносливости
                    *[other] у цели от {NATURALFIXED($min, 2)} до {NATURALFIXED($max, 2)} повреждений выносливости
                 }
    }

reagent-effect-condition-guidebook-unique-bloodstream-chem-threshold =
    { $max ->
        [2147483648] { $min ->
                        [1] есть как минимум {$min} реагент
                        *[other] есть как минимум {$min} реагента
                     }
        [1] { $min ->
               [0] есть не более {$max} реагента
               *[other] есть от {$min} до {$max} реагентов
            }
        *[other] { $min ->
                    [-1] есть не более {$max} реагентов
                    *[other] есть от {$min} до {$max} реагентов
                 }
    }

reagent-effect-condition-guidebook-typed-damage-threshold =
    { $inverse ->
        [true] у цели не более
        *[false] у цели как минимум
    } { $changes } урона