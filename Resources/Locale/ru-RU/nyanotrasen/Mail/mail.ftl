mail-recipient-mismatch = Имя получателя или должность не совпадает!
mail-invalid-access = Имя получателя и должность совпадают, но нужный доступ отсутствует!
mail-locked = Анти-воровской замок ещё не снят. Приложите ID карту получателя
mail-desc-far = Письмо. С этого расстояния вы не можете увидеть получателя.
mail-desc-close = Письмо адрессованное {CAPITALIZE($name)}, {$job}.
mail-desc-fragile = Оно имеет [color=red]красный штамп[/color].
mail-desc-priority = Анти-воровской замок автоматически включил [color=yellow]жёлтый режим[/color]. Лучше доставить это вовремя!
mail-desc-priority-inactive = Анти-воровской замок выключил [color=#886600]жёлтый режим[/color].
mail-unlocked = Анти-воровской замок разблокирован.
mail-unlocked-by-emag = Aнти-воровской замок издал странный звук.
mail-unlocked-reward = Анти-воровской замок разблокирован. {$bounty} было добавлено на логистический аккаунт.
mail-penalty-lock = АНТИ-ВОРОВСКОЙ ЗАМОК СЛОМАН. ЛОГИСТИЧЕСКИЙ АККАУНТ БЫЛ ОШТРАФОВАН НА {$credits}.
mail-penalty-fragile = ЦЕЛОСТНОСТЬ НАРУШЕНА. ЛОГИСТИЧЕСКИЙ АККАУНТ БЫЛ ОШТРАФОВАН НА {$credits}.
mail-penalty-expired = ВРЕМЯ ДОСТАВКИ ВЫШЛО. ЛОГИСТИЧЕСКИЙ АККАУНТ БЫЛ ОШТРАФОВАН НА {$credits}.
mail-item-name-unaddressed = письмо
mail-item-name-addressed = письмо ({$recipient})

command-mailto-description = Добавляет посылку в очередь на доставку сущности. Пример использования: `mailto 1234 5678 false false`. Содержимое целевого контейнера будет перенесено в реальную почтовую посылку.
### Frontier: add is-large description
command-mailto-help = Использование: {$command} <uid сущности получателя> <uid сущности контейнера> [хрупкая: true или false] [приоритетная: true или false] [большая: true или false, опционально]
command-mailto-no-mailreceiver = У целевой сущности-получателя отсутствует {$requiredComponent}.
command-mailto-no-blankmail = Прототип {$blankMail} не существует. Что-то пошло совсем не так. Обратитесь к программисту.
command-mailto-bogus-mail = У {$blankMail} отсутствует {$requiredMailComponent}. Что-то пошло совсем не так. Обратитесь к программисту.
command-mailto-invalid-container = У целевой сущности-контейнера отсутствует контейнер {$requiredContainer}.
command-mailto-unable-to-receive = Не удалось настроить целевую сущность-получателя для получения почты. Возможно, отсутствует ID.
command-mailto-no-teleporter-found = Не удалось сопоставить целевую сущность-получателя с почтовым телепортером ни одной станции. Возможно, получатель находится вне станции.
command-mailto-success = Успех! Почтовая посылка добавлена в очередь на следующую телепортацию через {$timeToTeleport} секунд.

command-mailnow = Немедленно заставляет все почтовые телепортеры доставить еще один раунд почты как можно скорее. Это не обойдет ограничение на недоставленную почту.
command-mailnow-help = Использование: {$command}
command-mailnow-success = Успех! Все почтовые телепортеры скоро доставят еще один раунд почты.