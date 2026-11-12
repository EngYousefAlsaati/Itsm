// ITSM: ترجمة الواجهة بين العربية والإنجليزية (تُحمَّل في رأس كل صفحة)
// اللغة الافتراضية العربية. عند اختيار English تُترجَم نصوص النظام الثابتة والرسائل والإشعارات،
// ولا تُترجَم النصوص التي يكتبها المستخدمون (عناوين التذاكر والتعليقات وغيرها).
(function () {
    var LANG = 'ar';
    try { LANG = localStorage.getItem('lang') === 'en' ? 'en' : 'ar'; } catch (e) { }

    window.__lang = LANG;
    window.toggleLang = function () {
        try { localStorage.setItem('lang', LANG === 'en' ? 'ar' : 'en'); } catch (e) { }
        location.reload();
    };

    function setButton() {
        var b = document.getElementById('langBtn');
        if (b) { b.textContent = LANG === 'en' ? 'العربية' : 'English'; }
    }
    document.addEventListener('DOMContentLoaded', setButton);

    if (LANG !== 'en') {
        window.__tr = function (s) { return s; };
        return;
    }

    var root = document.documentElement;
    root.classList.add('lang-en');
    root.setAttribute('lang', 'en');
    root.setAttribute('dir', 'ltr');

    var D = {
        "(بانتظار اعتماد مدير الاستقبال)": "(awaiting reception manager approval)",
        "+ إنشاء تذكرة جديدة": "+ Create a new ticket",
        "-- اختر الفني --": "-- Choose technician --",
        "-- اختر المشروع --": "-- Choose project --",
        "-- اختر مستوى التأثير --": "-- Choose impact level --",
        "8 أحرف على الأقل، وتحتوي على حرف ورقم.": "At least 8 characters, containing a letter and a digit.",
        "© 2026 - شبه الجزيرة لتقنية نظم المعلومات (SITCO)": "© 2026 - Shibh Al-Jazeera Information Systems Technology (SITCO)",
        "، وقد سُجّل هذا التأخير.": ", and this delay has been recorded.",
        "آخر دخول": "Last sign-in",
        "أبريل": "April",
        "أدخل اسمك الثلاثي": "Enter your full name",
        "أدخل بياناتك للمتابعة إلى حسابك": "Enter your details to continue to your account",
        "أدخل كلمة المرور الخاصة بالحساب": "Enter your account password",
        "أرسلت الحل وهو الآن بانتظار اعتماد مدير الاستقبال.": "You sent the solution and it is now awaiting reception manager approval.",
        "أزمنة الاستجابة والحل يجب أن تكون بالدقائق وأكبر من صفر.": "Response and resolution times must be in minutes and greater than zero.",
        "أشهر": "Months",
        "أضف جهة أولاً من «إدارة الجهات وشعاراتها».": "Add a client first from “Client management”.",
        "أضف عميلاً أولاً من «إدارة العملاء».": "Add a client first from “Client management”.",
        "أغسطس": "August",
        "أكتوبر": "October",
        "أيام": "Days",
        "أُعيد الحل إلى الفني لاستكمال المعالجة": "The solution was returned to the technician to continue working",
        "أُوقف العداد — بانتظار الاعتماد": "Counter stopped — awaiting approval",
        "إتاحة التقييم للموظفين ولمدراء الاستقبال أو إيقافه، وتحديد دورة التقييم (6 أشهر أو سنة)": "Enable or disable rating for employees and reception managers, and set the rating cycle (6 months or a year)",
        "إجراء": "Action",
        "إجمالي التذاكر": "Total tickets",
        "إدارة اتفاقيات مستوى الخدمة لكل مشروع وتعديل أزمنتها ووصفها بالعربية والإنجليزية": "Manage the service level agreement for each project and edit its times and description in Arabic and English",
        "إدارة العملاء": "Client management",
        "إدارة المستخدمين": "User management",
        "إدارة المستخدمين والعملاء (الجهات) والمشاريع": "Manage users, clients (organizations) and projects",
        "إدارة المستخدمين والعملاء والمشاريع واتفاقيات SLA (لمدير النظام)": "Manage users, clients, projects and SLA agreements (system administrator)",
        "إدارة المشاريع": "Project management",
        "إرجاع الحلول من صلاحية مدير الاستقبال فقط.": "Returning solutions is the reception manager's authority only.",
        "إرسال إلى": "Send to",
        "إرسال التعليق": "Send comment",
        "إرسال التقييم": "Submit rating",
        "إرسال الرد": "Send reply",
        "إرفاق الصور والملفات التوضيحية": "Attach images and supporting files",
        "إضافة": "Add",
        "إضافة عميل جديد": "Add a new client",
        "إضافة مستخدم جديد": "Add a new user",
        "إضافة مشروع جديد": "Add a new project",
        "إضافة موظفي جهة دفعة واحدة": "Add an organization's employees at once",
        "إضافة وتوليد كلمات المرور": "Add and generate passwords",
        "إظهار / إخفاء": "Show / hide",
        "إظهار كلمة المرور": "Show password",
        "إعدادات التقييم": "Rating settings",
        "إغلاق": "Close",
        "إغلاق التذكرة بنفسك (يعتمد مدير الاستقبال الحل أولاً)": "Close the ticket yourself (the reception manager approves the solution first)",
        "إلغاء": "Cancel",
        "إنشاء التذكرة": "Ticket created",
        "إنشاء تذاكر": "Create tickets",
        "إنشاء تذاكر (للموظف فقط)": "Create tickets (employee only)",
        "إنشاء تذكرة": "Create ticket",
        "إنشاء تذكرة جديدة": "Create a new ticket",
        "إنشاء تذكرة جديدة باختيار أحد مشاريع جهتك (المشروع إلزامي)": "Create a new ticket by choosing one of your organization's projects (project is mandatory)",
        "إنشاء تذكرة دون مشروع مسجّل لجهتك": "Create a ticket without a project registered for your organization",
        "إنشاء طلب خدمة جديد": "Create a new service request",
        "إيقاف": "Suspend",
        "إيقاف SLA": "SLA stopped",
        "إيقاف عداد SLA": "SLA counter stop",
        "إيقاف عداد الـ SLA:": "SLA counter stopped:",
        "اتفاقيات مستوى الخدمة": "Service level agreements",
        "اتفاقية SLA": "SLA agreement",
        "اتفاقية مستوى الخدمة": "Service level agreement",
        "اختر المشروع الذي يخصه الطلب.": "Choose the project this request belongs to.",
        "اختر جهة صالحة.": "Choose a valid client.",
        "اختر دوراً صحيحاً.": "Choose a valid role.",
        "اختر عميلاً صالحاً.": "Choose a valid client.",
        "اختر فنياً صالحاً.": "Choose a valid technician.",
        "اختر مستوى التأثير.": "Choose the impact level.",
        "اختر وحدة الزمن لكل خانة: دقيقة أو ساعة أو يوم.": "Choose the time unit for each field: minute, hour or day.",
        "اختر وحدة المدة (أيام أو أشهر).": "Choose the duration unit (days or months).",
        "ارسال الرسالة": "Send message",
        "اسم الجهة": "Organization name",
        "اسم العميل مطلوب.": "Client name is required.",
        "اسم المشروع": "Project name",
        "اسم المشروع طويل جداً.": "Project name is too long.",
        "اسم المشروع مطلوب.": "Project name is required.",
        "اسم الموظف": "Employee name",
        "اسم الموظف مطلوب": "Employee name is required",
        "اعتماد الحلول من صلاحية مدير الاستقبال فقط.": "Approving solutions is the reception manager's authority only.",
        "اعتماد حل الفني": "Approve technician's solution",
        "اعتماد حل الفني وإغلاق التذكرة، أو إرجاعه للفني مع السبب": "Approve the technician's solution and close the ticket, or return it to the technician with a reason",
        "اعتماد حلول الفنيين (لمدير الاستقبال وحده)": "Approve technicians' solutions (reception manager only)",
        "اعتمد مدير الاستقبال الحل — تم حل المشكلة وأُغلقت التذكرة": "The reception manager approved the solution — the problem is solved and the ticket is closed",
        "افتح": "Open",
        "اكتب اسمك ووسيلة التواصل قبل إرسال التقييم.": "Enter your name and contact details before submitting the rating.",
        "اكتب النص أولاً ثم اضغط الترجمة.": "Write the text first, then click Translate.",
        "اكتب الوصف بالعربية وبالإنجليزية (كل بند في سطر)، وزر الترجمة التلقائية يملأ اللغة الأخرى من اللغة التي كتبتها.": "Write the description in Arabic and English (one item per line); the automatic translation button fills the other language from the one you wrote.",
        "اكتب بريداً واحداً على الأقل (بريد في كل سطر).": "Enter at least one email (one email per line).",
        "اكتب تعليقاً أو استفساراً على التذكرة...": "Write a comment or inquiry about the ticket...",
        "اكتب تقرير الحل قبل الإرسال.": "Write the solution report before sending.",
        "اكتب ردك على التقييم": "Write your reply to the rating",
        "اكتب سبب إرجاع الحل للفني.": "Write the reason for returning the solution to the technician.",
        "اكتب سبب الرفض ليظهر للموظف": "Write the rejection reason so it appears to the employee",
        "اكتب ملاحظات بسيطة وسيقوم الذكاء الاصطناعي بتحسين صياغة الوصف...": "Write simple notes and the AI will improve the wording of the description...",
        "اكتب نص التعليق أولاً.": "Enter the comment text first.",
        "اكتب نص الرد أولاً.": "Enter the reply text first.",
        "الأدوار والصلاحيات": "Roles and permissions",
        "الإجراءات": "Actions",
        "الإدارة": "Administration",
        "الإشعارات": "Notifications",
        "الاتفاقية": "Agreement",
        "الاحترافية وحسن التعامل": "Professionalism and courtesy",
        "الاستفسار أو الاقتراح": "Inquiry or suggestion",
        "الاسم": "Name",
        "الاسم (اختياري)": "Name (optional)",
        "الاطلاع على اتفاقيات مستوى الخدمة": "View service level agreements",
        "الاطلاع على اتفاقيات مستوى الخدمة لجميع المشاريع": "View service level agreements for all projects",
        "الاطلاع على اتفاقيات مستوى الخدمة لمشاريع جهتك": "View service level agreements for your organization's projects",
        "الاطلاع على التذاكر الموجّهة إليك فقط": "View only the tickets assigned to you",
        "الاطلاع على التقارير والإحصائيات": "View reports and statistics",
        "الاطلاع على تقييمات الفنيين": "View technician ratings",
        "الاطلاع على جميع التذاكر": "View all tickets",
        "الاطلاع على جميع التذاكر وجميع المراسلات (قناة الموظف وقناة الفني)": "View all tickets and all conversations (employee channel and technician channel)",
        "الاطلاع على مراسلات الفني": "View the technician's conversations",
        "الاطلاع على مراسلات الموظف": "View the employee's conversations",
        "الالتزام بالإجراءات وسياسات العمل": "Compliance with procedures and work policies",
        "الالتزام بمهل SLA": "SLA compliance",
        "الالتزام بمهلة SLA (التذاكر المغلقة)": "SLA compliance (closed tickets)",
        "الالتزام بمهلة SLA (للمغلقة)": "SLA compliance (closed tickets)",
        "البداية": "Start",
        "البريد": "Email",
        "البريد الإلكتروني": "Email",
        "البريد الإلكتروني أو رقم الجوال (للتواصل)": "Email or mobile number (for contact)",
        "البريد الإلكتروني أو كلمة المرور غير صحيحة": "Incorrect email or password",
        "البريد الإلكتروني غير صالح.": "Invalid email address.",
        "البريد الإلكتروني للموظفين (بريد في كل سطر، ويمكن كتابة الاسم بعده بعد فاصلة)": "Employees' emails (one per line; a name can follow after a comma)",
        "البريد الإلكتروني مطلوب": "Email is required",
        "التالي": "Next",
        "التذاكر": "Tickets",
        "التذاكر المُنشأة في آخر 6 أشهر": "Tickets created in the last 6 months",
        "الترجمة التلقائية غير مفعّلة بعد (لم يُضبط مفتاح الخدمة في الإعدادات). اكتب النص باللغتين يدوياً.": "Automatic translation is not enabled yet (service key not configured in settings). Enter the text in both languages manually.",
        "التعاون مع الفريق": "Teamwork and cooperation",
        "التعليق": "Comment",
        "التفاصيل": "Details",
        "التقييم": "Rating",
        "التقييم الدوري: كل 6 أشهر أو سنوياً (يحدد مدير النظام الدورة) يقيّم الموظف ومدير الاستقبال أداء الفني ببنود عشرة من 1 إلى 5، متى أتاح مدير النظام ذلك لفئتهم. ويطّلع الجميع على التقييمات.": "Periodic rating: every 6 months or yearly (the system administrator sets the cycle), the employee and the reception manager rate the technician's performance on ten criteria from 1 to 5, once the system administrator enables it for their role. Everyone can see the ratings.",
        "التقييم غير مفعّل لفئتك حالياً. يتيحه مدير النظام.": "Rating is not enabled for your role at the moment. The system administrator enables it.",
        "التقييم غير مفعّل لفئتك حالياً، يتيحه مدير النظام.": "Rating is not enabled for your role at the moment; the system administrator enables it.",
        "التقييم متاح للموظف ومدير الاستقبال فقط.": "Rating is available to employees and reception managers only.",
        "التقييمات": "Ratings",
        "التواصل والتنسيق": "Communication and coordination",
        "الجهة": "Client",
        "الجهة المختارة غير صالحة.": "The selected client is invalid.",
        "الجهة تخص الموظفين طالبي الخدمة فقط.": "The client applies to employees (service requesters) only.",
        "الجهة مطلوبة": "Organization is required",
        "الجهة:": "Organization:",
        "الحالة": "Status",
        "الحد الأقصى 50 بريداً في المرة الواحدة.": "Maximum 50 emails at a time.",
        "الحد الأقصى للمدة 120 شهراً.": "Maximum duration is 120 months.",
        "الحد الأقصى للمدة 3650 يوماً.": "Maximum duration is 3650 days.",
        "الحساب": "Account",
        "الحل": "Solution",
        "الحل:": "Solution:",
        "الخطورة": "Severity",
        "الدور": "Role",
        "الرجاء كتابة تفاصيل أولية للمشكلة في حقل الوصف ليقوم الذكاء الاصطناعي بصياغتها.": "Please write some initial details of the problem in the description field for the AI to phrase them.",
        "الرد على تقييمات الفنيين وحذفها، ورؤية وسيلة تواصل المقيِّم": "Reply to technician ratings and delete them, and see the evaluator's contact details",
        "الرضا العام عن الخدمة": "Overall satisfaction with the service",
        "الرقم": "No.",
        "السابق": "Previous",
        "السماح للموظفين بتقييم الفنيين": "Allow employees to rate technicians",
        "السماح لمدراء الاستقبال بتقييم الفنيين": "Allow reception managers to rate technicians",
        "الشعار": "Logo",
        "الشعار (PNG أو JPG أو WEBP، حتى 2 ميجابايت)": "Logo (PNG, JPG or WEBP, up to 2 MB)",
        "الضغط على «تم حل المشكلة» مع التقرير والملاحظات والمرفقات، فيتوقف عدّاد SLA": "Click “Problem solved” with the report, notes and attachments, which stops the SLA counter",
        "العميل": "Client",
        "العميل هو الجهة طالبة الخدمة. عند دخول موظفيه يظهر شعاره بجانب شعار شبه الجزيرة في الشريط العلوي.": "A client is the organization requesting the service. When its employees sign in, its logo appears next to the Shibh Al-Jazeera logo in the top bar.",
        "العنوان": "Title",
        "العودة": "Back",
        "العودة إلى إدارة المستخدمين": "Back to user management",
        "العودة إلى قائمة التذاكر": "Back to tickets list",
        "العودة للقائمة": "Back to list",
        "الفترة": "Period",
        "الفني": "Technician",
        "الفني (قناة الفني)": "Technician (technician channel)",
        "الفني / المهندس المختص": "Specialist technician / engineer",
        "الفني المختار غير صالح.": "The selected technician is invalid.",
        "الفني المختص": "Specialist technician",
        "الفني المسؤول": "Responsible technician",
        "الفني المسؤول عن الحل:": "Technician responsible for the solution:",
        "الفني يعالج ويضغط «تم حل المشكلة» مع التقرير ← يتوقف العدّاد وتنتظر التذكرة اعتماد مدير الاستقبال": "The technician works on it and clicks “Problem solved” with the report ← the counter stops and the ticket awaits the reception manager's approval",
        "القائمة": "Menu",
        "القائمة الرئيسية": "Main menu",
        "المتوسط العام": "Overall average",
        "المحادثات وسجل التذكرة (قناة الموظف وقناة الفني)": "Conversations and ticket log (employee channel and technician channel)",
        "المحادثة مع مدير الاستقبال وسجل التذكرة": "Conversation with the reception manager and ticket log",
        "المدة": "Duration",
        "المدينة": "City",
        "المرفقات": "Attachments",
        "المرفقات (صور، مستندات)": "Attachments (images, documents)",
        "المرفقات المرتبطة:": "Related attachments:",
        "المزيد": "More",
        "المسموح لك": "Allowed for you",
        "المشاريع": "Projects",
        "المشروع": "Project",
        "المشروع:": "Project:",
        "المقيِّم": "Evaluator",
        "المقيِّم:": "Evaluator:",
        "المنفّذ": "Performed by",
        "المهارة والكفاءة التقنية": "Technical skill and competence",
        "المورّد / المدير": "Reception manager / Administrator",
        "الموظف": "Employee",
        "الموظف (قناة الموظف)": "Employee (employee channel)",
        "الموظف مقدّم الطلب": "Requesting employee",
        "الموظف ينشئ التذكرة ويختار مشروعه ←": "The employee creates the ticket and chooses their project ←",
        "الموظف:": "Employee:",
        "الموظفون": "Employees",
        "الموعد النهائي": "Deadline",
        "الموعد النهائي SLA": "SLA deadline",
        "الموعد النهائي للـ SLA": "SLA deadline",
        "النتيجة": "Result",
        "النص طويل جداً للترجمة (الحد 3000 حرف).": "The text is too long to translate (limit 3000 characters).",
        "النظام": "System",
        "النهاية": "End",
        "النوع": "Type",
        "الوحدة": "Unit",
        "الوصف": "Description",
        "الوصف الإنجليزي طويل جداً (الحد 2000 حرف).": "The English description is too long (limit 2000 characters).",
        "الوصف بالعربية (كل بند في سطر)": "Description in Arabic (one item per line)",
        "الوصف:": "Description:",
        "الوضع الليلي": "Dark mode",
        "الوقت": "Time",
        "الوقت المتبقي لحل التذكرة ضمن المهلة.": "Remaining time to solve the ticket within the deadline.",
        "انتبه:": "Note:",
        "انسخها أو اطبع الصفحة وسلّمها لأصحابها الآن، وننصح كل موظف بتغييرها من «تغيير كلمة المرور» بعد أول دخول.": "Copy them or print the page and hand them to their owners now; we advise each employee to change it from “Change password” after the first sign-in.",
        "بإرسال التقرير يتوقف عدّاد SLA وتنتقل التذكرة إلى مدير الاستقبال لاعتماد الحل.": "By sending the report the SLA counter stops and the ticket moves to the reception manager to approve the solution.",
        "بانتظار الموافقة": "Pending approval",
        "بانتظار تقييمك": "Awaiting your rating",
        "بانتظار تقييمك في هذه الدورة": "Awaiting your rating in this cycle",
        "بانتظار قرارك": "Awaiting your decision",
        "بانتظار قرارك:": "Awaiting your decision:",
        "بحث": "Search",
        "بحث بالرقم أو العنوان أو الوصف أو الفني أو الجهة": "Search by number, title, description, technician or organization",
        "بدء SLA": "SLA start",
        "بدء عداد SLA": "SLA counter start",
        "بدء عداد الـ SLA:": "SLA counter start:",
        "بدون جهة": "No client",
        "بدون شعار": "No logo",
        "بريد إلكتروني غير صالح": "Invalid email address",
        "بريد غير صالح": "Invalid email",
        "بعد انقضاء زمن الاستجابة": "after the response time elapsed",
        "بيانات التذكرة": "Ticket details",
        "بيانات التواصل": "Contact details",
        "بيانات مقدم الطلب": "Requester details",
        "بياناتك": "Your details",
        "تأكيد كلمة المرور الجديدة": "Confirm new password",
        "تاريخ الإغلاق": "Closing date",
        "تاريخ الإنشاء": "Created at",
        "تاريخ البداية": "Start date",
        "تاريخ البداية غير صالح.": "Invalid start date.",
        "تاريخ النهاية": "End date",
        "تاريخ النهاية (يُحسب تلقائياً)": "End date (calculated automatically)",
        "تجاوز زمن الاستجابة": "Exceeded the response time",
        "تجاوزت التذكرة مهلة SLA": "The ticket exceeded the SLA deadline",
        "تجاوزت التذكرة مهلة SLA (ما زالت قيد المعالجة)": "The ticket exceeded the SLA deadline (still in progress)",
        "تجاوزت المهلة بـ": "Exceeded the deadline by",
        "تحديد الكل كمقروء": "Mark all as read",
        "تحديد مستوى التأثير أو مهلة SLA": "Set the impact level or SLA deadline",
        "تحديد مستوى التأثير للتذكرة، فيبدأ عدّاد SLA وفق اتفاقية مشروعها": "Set the ticket's impact level, which starts the SLA counter according to its project's agreement",
        "تحميل / عرض": "Download / view",
        "تذاكر مرفوضة": "Rejected tickets",
        "تذاكر نشطة متأخرة عن SLA": "Active tickets late against SLA",
        "تذكرة. افتح «قائمة التذاكر» من القائمة الجانبية.": "ticket(s). Open “Tickets” from the side menu.",
        "ترجمة إلى الإنجليزية ⟶": "Translate to English ⟶",
        "تسجيل الدخول": "Sign in",
        "تسجيل الدخول - ITSM": "Sign in - ITSM",
        "تسجيل خروج": "Sign out",
        "تسجيل دخول": "Sign in",
        "تصنيفك في النظام": "Your classification in the system",
        "تظهر هذه المرة فقط": "appear this time only",
        "تعديل": "Edit",
        "تعديل اتفاقية مستوى الخدمة": "Edit service level agreement",
        "تعديل الرد": "Edit reply",
        "تعديل المشروع": "Edit project",
        "تعذّر إرسال الرسالة، حاول مرة أخرى أو تواصل عبر الجوال.": "Could not send the message. Try again or contact us by phone.",
        "تعذّر الاتصال بالخادم.": "Could not connect to the server.",
        "تعذّر الاتصال بخدمة الترجمة.": "Could not connect to the translation service.",
        "تعذّر تحميل مكتبة الرسوم (تحقق من الاتصال بالإنترنت).": "Could not load the charts library (check your internet connection).",
        "تعذّرت الترجمة.": "Translation failed.",
        "تعليق": "Comment",
        "تغيير كلمة المرور": "Change password",
        "تفاصيل البنود العشرة": "Details of the ten criteria",
        "تفاصيل التذكرة": "Ticket details",
        "تفعيل": "Activate",
        "تقرير التذاكر": "Tickets report",
        "تقرير الحل": "Solution report",
        "تقرير الفني": "Technician report",
        "تقرير حل المشكلة": "Problem solution report",
        "تقييم الفني": "Rate technician",
        "تقييم الفنيين": "Rate technicians",
        "تقييم الفنيين تقييماً دورياً عاماً (كل 6 أشهر أو سنوياً) ببنود عشرة، متى أتاح مدير النظام ذلك": "Rate technicians with a general periodic evaluation (every 6 months or yearly) on ten criteria, once the system administrator enables it",
        "تقييم دوري عام لأداء الفني (وليس على تذكرة بعينها)، مرة في كل دورة. الدورة الحالية:": "General periodic evaluation of the technician's performance (not on a specific ticket), once per cycle. Current cycle:",
        "تقييم عام لأداء الفني خلال هذه الدورة، وتقيّمه مرة واحدة فقط فيها.": "General evaluation of the technician's performance during this cycle; you can rate once only.",
        "تقييمات الفنيين": "Technician ratings",
        "تقييمات الفنيين والمهندسين": "Technician and engineer ratings",
        "تم إرجاع الحل إلى الفني، واستؤنف عدّاد SLA.": "The solution was returned to the technician, and the SLA counter has resumed.",
        "تم إرسال الحل إلى مدير الاستقبال للاعتماد، وتوقف عدّاد SLA.": "The solution was sent to the reception manager for approval, and the SLA counter has stopped.",
        "تم إرسال رسالتك بنجاح، شكراً لتواصلك.": "Your message was sent successfully. Thank you for contacting us.",
        "تم اعتماد الحل وإغلاق التذكرة.": "Solution approved and ticket closed.",
        "تم البت في هذه التذكرة مسبقاً.": "A decision has already been made on this ticket.",
        "تم الحل بانتظار الاعتماد": "Solved — awaiting approval",
        "تم تغيير كلمة المرور بنجاح.": "Password changed successfully.",
        "تم حذف التقييم.": "Rating deleted.",
        "تم حفظ إعدادات التقييم.": "Rating settings saved.",
        "تم حفظ اتفاقية مستوى الخدمة.": "Service level agreement saved.",
        "تم حفظ الرد.": "Reply saved.",
        "تم حفظ بيانات العميل.": "Client details saved.",
        "تم حفظ بيانات المشروع.": "Project details saved.",
        "تم حل المشكلة": "Problem solved",
        "تم رفض التذكرة.": "Ticket rejected.",
        "تم قبول التذكرة وتوجيه الفني، وبدأ عدّاد SLA.": "Ticket accepted and technician assigned; the SLA counter has started.",
        "تمت الإضافة": "Added",
        "تواصل معنا": "Contact us",
        "توجيه الفني / المهندس المختص": "Assign the specialist technician / engineer",
        "توزيع التذاكر حسب الحالة": "Tickets by status",
        "توقف عدّاد SLA": "SLA counter stopped",
        "تُسجَّل جهتك وبريدك تلقائياً من حسابك، ولا حاجة لاختيارهما.": "Your organization and email are recorded automatically from your account; there is no need to choose them.",
        "تُولَّد لكل موظف كلمة مرور عشوائية قوية تظهر لك مرة واحدة فقط.": "A strong random password is generated for each employee and shown to you once only.",
        "ثابتة": "are fixed",
        "جاري تحسين الوصف...": "Improving the description...",
        "جارٍ": "Ongoing",
        "جارٍ الإرسال...": "Sending...",
        "جارٍ الترجمة...": "Translating...",
        "جديد": "New",
        "جهة هذا الحساب موقوفة. تواصل مع مدير النظام.": "This account's client is suspended. Contact the system administrator.",
        "جودة الحلول المقدّمة": "Quality of solutions",
        "جيد": "Good",
        "جيد جداً": "Very good",
        "حالة التذكرة": "Ticket status",
        "حالة التذكرة:": "Ticket status:",
        "حتى 5 ملفات، بحجم أقصى 10 ميجابايت للملف (صور، PDF، Word، Excel، نص).": "Up to 5 files, maximum 10 MB per file (images, PDF, Word, Excel, text).",
        "حجم الشعار يتجاوز 2 ميجابايت.": "The logo exceeds 2 MB.",
        "حدث": "Event",
        "حذف التقييم": "Delete rating",
        "حذف التقييم بواسطة مدير النظام": "Rating deleted by the system administrator",
        "حرج": "Critical",
        "حرجة": "Critical",
        "حسابك": "Your account",
        "حفظ": "Save",
        "حفظ الإعدادات": "Save settings",
        "حفظ الاتفاقية": "Save agreement",
        "حفظ كلمة المرور": "Save password",
        "حفظ وإرسال التذكرة": "Save and submit ticket",
        "خلال": "Within",
        "داخلي": "Internal",
        "دخول للنظام": "Sign in to the system",
        "دقيقة": "minutes",
        "دورة التقييم": "Rating cycle",
        "ديسمبر": "December",
        "راجع تقرير الفني أعلاه. باعتماد الحل تُغلق التذكرة ويظهر للموظف «تم حل المشكلة»، أو أعِدها للفني مع السبب فيستأنف العدّاد.": "Review the technician's report above. Approving the solution closes the ticket and the employee sees “Problem solved”, or return it to the technician with a reason and the counter resumes.",
        "رد مدير النظام:": "System administrator's reply:",
        "رفض التذاكر من صلاحية مدير الاستقبال فقط.": "Rejecting tickets is the reception manager's authority only.",
        "رفض التذكرة": "Ticket rejected",
        "رفض التذكرة (السبب مطلوب)": "Reject ticket (reason required)",
        "رفع الفني تقرير الحل وأوقف عدّاد SLA — بانتظار اعتماد مدير الاستقبال": "The technician submitted the solution report and stopped the SLA counter — awaiting reception manager approval",
        "رفع تقرير الحل (للفني المُوجَّه إليه)": "Submit the solution report (for the assigned technician)",
        "رفع تقرير الحل وإيقاف عداد SLA وإغلاق التذكرة": "Solution report submitted, SLA counter stopped and ticket closed",
        "رقم التذكرة": "Ticket No.",
        "رقم الجوال": "Mobile number",
        "رقم الجوال مطلوب": "Mobile number is required",
        "رقم المشروع": "Project number",
        "رقم المشروع طويل جداً.": "Project number is too long.",
        "رقم المشروع مسجّل لمشروع آخر.": "Project number is registered for another project.",
        "رقم المشروع مسجّل مسبقاً.": "Project number is already registered.",
        "رقم المشروع مطلوب.": "Project number is required.",
        "زمن الاستجابة": "Response time",
        "زمن الاستجابة (من إخطار العميل)": "Response time (from client notification)",
        "زمن الاستجابة:": "Response time:",
        "زمن الحل": "Resolution time",
        "زمن الحل (من وقت الاستجابة الأولية)": "Resolution time (from initial response)",
        "س": "h",
        "ساعات": "hours",
        "ساعة": "hours",
        "ساعة واحدة": "1 hour",
        "ساعتان": "2 hours",
        "سبب إرجاع الحل للفني": "Reason for returning the solution to the technician",
        "سبب الرفض": "Rejection reason",
        "سبب الرفض مطلوب.": "The rejection reason is required.",
        "سبتمبر": "September",
        "سجل التذكرة والمحادثة": "Ticket log and conversation",
        "سجّل الدخول لعرض الدور والصلاحيات الخاصة بك.": "Sign in to see your role and permissions.",
        "سرعة الاستجابة للتذاكر": "Ticket response speed",
        "شكراً، تم حفظ تقييمك ويراه جميع المستخدمين.": "Thank you, your rating has been saved and is visible to all users.",
        "شهر": "month",
        "صيغة الشعار غير مسموحة. المسموح: PNG أو JPG أو WEBP.": "Logo format not allowed. Allowed: PNG, JPG or WEBP.",
        "ضعيف": "Poor",
        "ضمن الاتفاقية": "Within the agreement",
        "ضمن المهلة": "Within deadline",
        "ضمن المهلة (جارية)": "Within deadline (in progress)",
        "ضمن زمن الاستجابة": "within response time",
        "عالي": "High",
        "عالية": "High",
        "عالٍ": "High",
        "عدد التذاكر": "Number of tickets",
        "عدد التقييمات": "Number of ratings",
        "عدد المنجزة:": "Completed:",
        "عدد غير المنجزة:": "Incomplete:",
        "عرض تقييمات:": "Show ratings for:",
        "عرض كل الإشعارات": "View all notifications",
        "عنوان الطلب": "Request title",
        "عنوان الطلب مطلوب": "Request title is required",
        "عنوان الطلب:": "Request title:",
        "غير محددة": "Not set",
        "غير مسموح لك": "Not allowed for you",
        "غير مصرّح.": "Not authorized.",
        "غير منجزة": "Incomplete",
        "فبراير": "February",
        "فني": "Technician",
        "فني / مهندس": "Technician / Engineer",
        "في نافذة الطباعة اختر الوجهة «Save as PDF» أو «حفظ كـ PDF».": "In the print window choose the destination “Save as PDF”.",
        "قائمة التذاكر": "Tickets",
        "قبول التذاكر أو رفضها أو توجيه الفني (لمدير الاستقبال وحده)": "Accept, reject tickets or assign the technician (reception manager only)",
        "قبول التذاكر وتوجيهها من صلاحية مدير الاستقبال فقط.": "Accepting and assigning tickets is the reception manager's authority only.",
        "قبول التذاكر ورفضها وتوجيه الفني واعتماد الحلول من صلاحية": "Accepting, rejecting, assigning technicians and approving solutions is the authority of",
        "قبول التذكرة أو رفضها (مع ذكر سبب الرفض)": "Accept or reject tickets (with a rejection reason)",
        "قبول التذكرة أو رفضها أو توجيه الفني": "Accept or reject tickets or assign the technician",
        "قبول التذكرة أو رفضها أو توجيه الفني أو تحديد مستوى التأثير": "Accept, reject, assign the technician or set the impact level",
        "قبول التذكرة وتوجيه الفني (يبدأ عداد SLA)": "Accept ticket and assign technician (starts the SLA counter)",
        "قرار مدير الاستقبال": "Reception manager decision",
        "قناة الفني": "Technician channel",
        "قناة الموظف": "Employee channel",
        "قيد المعالجة": "In progress",
        "قيّم جميع البنود العشرة بدرجة من 1 إلى 5.": "Rate all ten criteria from 1 to 5.",
        "قيّم كل بند من 1 (ضعيف) إلى 5 (ممتاز)": "Rate each criterion from 1 (poor) to 5 (excellent)",
        "قيّمت هذا الفني مسبقاً في هذه الدورة.": "You have already rated this technician in this cycle.",
        "قُبلت التذكرة بعد": "Ticket accepted after",
        "كل 6 أشهر (يناير–يونيو، يوليو–ديسمبر)": "Every 6 months (January–June, July–December)",
        "كل التذاكر": "All tickets",
        "كل الحالات": "All statuses",
        "كل الدورات": "All cycles",
        "كل مستويات التأثير": "All impact levels",
        "كلمات المرور في الجدول أدناه": "The passwords in the table below",
        "كلمة المرور": "Password",
        "كلمة المرور الجديدة": "New password",
        "كلمة المرور الجديدة وتأكيدها غير متطابقتين.": "The new password and its confirmation do not match.",
        "كلمة المرور الحالية": "Current password",
        "كلمة المرور الحالية غير صحيحة.": "The current password is incorrect.",
        "كلمة المرور المؤقتة": "Temporary password",
        "كلمة المرور مطلوبة.": "Password is required.",
        "كلمة المرور: 8 أحرف على الأقل، وتحتوي على حرف ورقم. والجهة تُحتسب للموظفين فقط.": "Password: at least 8 characters, containing a letter and a digit. The client applies to employees only.",
        "كلمة مرور جديدة": "New password",
        "لا توجد أحداث أو تعليقات بعد.": "There are no events or comments yet.",
        "لا توجد أحداث مسجّلة.": "No events recorded.",
        "لا توجد إشعارات بعد.": "No notifications yet.",
        "لا توجد إشعارات.": "No notifications.",
        "لا توجد تذاكر بعد.": "No tickets yet.",
        "لا توجد تذاكر مطابقة.": "No matching tickets.",
        "لا توجد تذاكر مغلقة بعد.": "No closed tickets yet.",
        "لا توجد تقييمات بعد.": "No ratings yet.",
        "لا توجد تقييمات في هذه الفترة.": "There are no ratings in this period.",
        "لا توجد مرفقات لهذه التذكرة.": "There are no attachments for this ticket.",
        "لا توجد مرفقات.": "No attachments.",
        "لا توجد مشاريع بعد.": "There are no projects yet.",
        "لا توجد مشاريع مسجّلة لجهتك. تواصل مع مدير النظام.": "No projects are registered for your organization. Contact the system administrator.",
        "لا يمكن إنشاء تذكرة الآن.": "A ticket cannot be created now.",
        "لا يمكنك إيقاف حسابك الحالي.": "You cannot suspend your own account.",
        "لا يوجد عملاء بعد. أضف أول عميل من النموذج أعلاه.": "There are no clients yet. Add the first client using the form above.",
        "لا يوجد فني مُسند لهذه التذكرة بعد.": "No technician is assigned to this ticket yet.",
        "لا يوجد فنيون بانتظار تقييمك في هذه الدورة.": "There are no technicians awaiting your rating in this cycle.",
        "لا يوجد فنيون بعد.": "There are no technicians yet.",
        "لا يوجد مشروع جارٍ مسجّل لجهتك في النظام. تواصل مع مدير النظام لتسجيل مشروعك، ثم أعد المحاولة.": "There is no ongoing project registered for your organization in the system. Contact the system administrator to register your project, then try again.",
        "لا يوجد نص للترجمة.": "There is no text to translate.",
        "لعرض جدول التذاكر اضغط على «قائمة التذاكر» من القائمة الجانبية.": "To view the tickets table, click “Tickets” in the side menu.",
        "لكل مشروع اتفاقية خاصة تحدد، لكل مستوى تأثير، زمن الاستجابة وزمن الحل وجهة التصعيد.": "Each project has its own agreement that defines, for each impact level, the response time and resolution time.",
        "لكل مشروع اتفاقية مستوى خدمة خاصة به تحدد، لكل مستوى تأثير، زمن الاستجابة وزمن الحل.": "Each project has its own service level agreement that defines, for each impact level, the response time and resolution time.",
        "لكل مشروع اتفاقية مستوى خدمة خاصة به، ويحدد مدير الاستقبال مستوى التأثير عند قبول الطلب.": "Each project has its own service level agreement, and the reception manager sets the impact level when accepting the request.",
        "للاستفسارات والاقتراحات": "For inquiries and suggestions",
        "للاطلاع عليها.": "to view them.",
        "لم يبدأ": "Not started",
        "لم يبدأ بعد": "Not started yet",
        "لم يتم التوجيه بعد": "Not assigned yet",
        "لم يُحدَّد": "Not set",
        "لوحة التحكم": "Dashboard",
        "ليست لديك صلاحية لعرض هذه التذكرة.": "You do not have permission to view this ticket.",
        "ليست لديك صلاحية لفتح هذا المرفق.": "You do not have permission to open this attachment.",
        "ليست لديك صلاحية للاطلاع على هذه الاتفاقية.": "You do not have permission to view this agreement.",
        "ما زالت التذكرة مفتوحة والفني يواصل معالجتها، ويستمر العدّاد في احتساب مدة التأخير حتى الحل.": "The ticket is still open and the technician continues working on it; the counter keeps counting the delay until it is solved.",
        "مارس": "March",
        "مايو": "May",
        "متأخرة": "Late",
        "متأخرة (جارية)": "Late (in progress)",
        "متابعة حالة تذاكرك": "Follow the status of your tickets",
        "متبقي": "Remaining",
        "متوسط": "Medium",
        "متوسط زمن الحل": "Average resolution time",
        "متوسطة": "Medium",
        "محتوى الملف لا يطابق امتداده.": "The file content does not match its extension.",
        "مدة المشروع يجب أن تكون 1 على الأقل.": "Project duration must be at least 1.",
        "مدن": "MODON",
        "مدير الاستقبال": "Reception manager",
        "مدير الاستقبال يعتمد الحل ←": "The reception manager approves the solution ←",
        "مدير الاستقبال يقبلها ويحدد مستوى التأثير ويوجّه الفني ←": "The reception manager accepts it, sets the impact level and assigns the technician ←",
        "مدير النظام": "System administrator",
        "مراسلة الموظف ومراسلة الفني كلٌّ في قناته": "Message the employee and the technician, each in their own channel",
        "مراسلة مدير الاستقبال داخل التذكرة (لا يرى الفني هذه المراسلة)": "Message the reception manager inside the ticket (the technician does not see this conversation)",
        "مراسلة مدير الاستقبال داخل التذكرة (لا يرى الموظف هذه المراسلة)": "Message the reception manager inside the ticket (the employee does not see this conversation)",
        "مرة كل سنة": "Once a year",
        "مرحباً بك في": "Welcome to",
        "مرفقات / صور توضيحية (اختياري)": "Attachments / supporting images (optional)",
        "مرفوضة": "Rejected",
        "مسار التذكرة": "Ticket workflow",
        "مستوى التأثير": "Impact level",
        "مستوى التأثير (يحدد مهلة SLA من اتفاقية المشروع)": "Impact level (sets the SLA deadline from the project agreement)",
        "مستوى التأثير:": "Impact level:",
        "مستويات التأثير الأربعة": "The four impact levels",
        "مسجّل مسبقاً": "Already registered",
        "مسح": "Clear",
        "معرفة أن المشكلة حُلّت بعد أن يعتمد مدير الاستقبال حل الفني": "Know that the problem is solved after the reception manager approves the technician's solution",
        "مغلقة": "Closed",
        "مقبول": "Acceptable",
        "مقفل مؤقتاً": "Temporarily locked",
        "مكرر في القائمة": "Duplicate in the list",
        "ملاحظات (اختياري)": "Notes (optional)",
        "ملاحظات الفني": "Technician notes",
        "ملاحظات الفني:": "Technician notes:",
        "ملخص تقييم الفنيين": "Technician ratings summary",
        "ملف الشعار غير صالح.": "The logo file is invalid.",
        "ممتاز": "Excellent",
        "من إخطار العميل": "of client notification",
        "من الموظفين": "From employees",
        "من مدراء الاستقبال": "From reception managers",
        "من وقت الاستجابة الأولية": "from initial response",
        "منتهٍ": "Ended",
        "منخفض": "Low",
        "منخفضة": "Low",
        "منشآت": "Monsha'at",
        "مهامك النشطة": "Your active tasks",
        "مهلة المعالجة (SLA)": "Processing deadline (SLA)",
        "موجّهة للفني": "Assigned to technician",
        "مورّد": "Reception manager",
        "موظف": "Employee",
        "موعد انتهاء الـ SLA:": "SLA deadline:",
        "موقوف": "Suspended",
        "نتيجة SLA": "SLA result",
        "نتيجة إضافة الموظفين": "Result of adding employees",
        "نسبة المهام المنجزة": "Completed tasks rate",
        "نسبة المهام غير المنجزة": "Incomplete tasks rate",
        "نسيت كلمة المرور؟ تواصل مع مدير النظام.": "Forgot your password? Contact the system administrator.",
        "نشط": "Active",
        "نظام خدمات الدعم الفني": "Technical Support Services System",
        "نوفمبر": "November",
        "هذا البريد مسجّل مسبقاً.": "This email is already registered.",
        "هذا الحساب موقوف. تواصل مع مدير النظام.": "This account is suspended. Contact the system administrator.",
        "هذا العميل مسجّل مسبقاً.": "This client is already registered.",
        "هذه التذكرة ليست بانتظار الاعتماد.": "This ticket is not awaiting approval.",
        "هذه الصفحة لمدير النظام فقط.": "This page is for the system administrator only.",
        "هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية": "Imam Abdulaziz bin Mohammed Royal Reserve Development Authority",
        "و": "and",
        "والصيانة المتكامل": "and Integrated Maintenance",
        "وحده.": "only.",
        "وزارة الصحة": "Ministry of Health",
        "وصف المستوى طويل جداً (الحد 2000 حرف).": "The level description is too long (limit 2000 characters).",
        "وصف المشكلة": "Problem description",
        "وصف المشكلة مطلوب": "Problem description is required",
        "وضوح تقارير الحل وتوثيقها": "Clarity and documentation of solution reports",
        "وقت الإنشاء:": "Created at:",
        "وقت بدء الـ SLA": "SLA start time",
        "ولا تُعدَّل، ويُعدَّل وصفها وأزمنتها فقط.": "and cannot be edited; only their description and times can be edited.",
        "ولا يمكن استرجاعها لاحقاً.": "and cannot be recovered later.",
        "ويبدأ عدّاد SLA بزمن الحل في اتفاقية المشروع، أو يرفضها مع السبب": "and the SLA counter starts with the resolution time in the project agreement, or rejects it with a reason",
        "ويظهر للموظف «تم حل المشكلة»، أو يعيده للفني فيستأنف العدّاد": "and the employee sees “Problem solved”, or returns it to the technician and the counter resumes",
        "يجب ألا تقل كلمة المرور عن 8 أحرف.": "The password must be at least 8 characters.",
        "يجب أن تحتوي كلمة المرور على حرف واحد على الأقل.": "The password must contain at least one letter.",
        "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.": "The password must contain at least one digit.",
        "يجب أن تختلف كلمة المرور الجديدة عن الحالية.": "The new password must be different from the current one.",
        "يجب أن يبقى مدير نظام واحد نشط على الأقل.": "At least one active system administrator must remain.",
        "يحدد مدير الاستقبال مستوى التأثير عند قبول التذكرة، ويبدأ عدّاد SLA للفني وفق زمن الحل المذكور لهذا المستوى في اتفاقية هذا المشروع.": "The reception manager sets the impact level when accepting the ticket, and the technician's SLA counter starts according to the resolution time of that level in this project's agreement.",
        "يحدده مدير الاستقبال عند القبول": "Set by the reception manager upon acceptance",
        "يختار الموظف عند إنشاء التذكرة مشروعاً من مشاريع عميله. يُحسب تاريخ نهاية المشروع تلقائياً من تاريخ البداية والمدة.": "The employee chooses a project of their client when creating a ticket. The project end date is calculated automatically from the start date and duration.",
        "يصل تعليقك إلى مدير الاستقبال ومدير النظام.": "Your comment reaches the reception manager and the system administrator.",
        "يظهر اسمك مع تقييمك للجميع، أما وسيلة التواصل فلا يراها إلا مدير النظام.": "Your name appears with your rating to everyone, while the contact detail is visible only to the system administrator.",
        "يعادل:": "Equals:",
        "يقيّم الموظف ومدير الاستقبال كلٌّ على حدة ببنود عشرة، مرة واحدة لكل فني في كل دورة. ويقيّم الموظف الفنيين الذين أغلقوا له تذاكر خلال الدورة.": "The employee and the reception manager each rate on ten criteria, once per technician per cycle. The employee rates the technicians who closed tickets for them during the cycle.",
        "يمكنك تقييم الفنيين الذين أغلقوا لك تذاكر خلال هذه الدورة فقط.": "You can only rate technicians who closed tickets for you during this cycle.",
        "يناير": "January",
        "يوجد عميل آخر بنفس الاسم.": "Another client has the same name.",
        "يوليو": "July",
        "يوم": "day",
        "يوم واحد": "1 day",
        "يومان": "2 days",
        "يوماً": "days",
        "يونيو": "June",
        "— أرسل الفني الحل وبانتظار اعتماد مدير الاستقبال.": "— the technician submitted the solution and it awaits reception manager approval.",
        "— الحالية": "— current",
        "↩ إرجاع للفني": "↩ Return to technician",
        "⏱️ حُلّت التذكرة بعد انتهاء مهلة SLA بـ": "⏱️ The ticket was solved after the SLA deadline by",
        "⏱️ متابعة مهلة SLA": "⏱️ SLA deadline tracking",
        "⏳ اقتربت مهلة SLA من الانتهاء": "⏳ The SLA deadline is about to expire",
        "⚠️ تجاوزت التذكرة مهلة SLA": "⚠️ The ticket exceeded the SLA deadline",
        "✅ التذكرة ضمن مهلة SLA": "✅ The ticket is within the SLA deadline",
        "✅ تم حل المشكلة.": "✅ The problem is solved.",
        "✅ حُلّت التذكرة ضمن مهلة SLA.": "✅ The ticket was solved within the SLA deadline.",
        "✏️ تعديل الاتفاقية": "✏️ Edit agreement",
        "✔ اعتماد الحل": "✔ Approve solution",
        "✔ تم حل المشكلة": "✔ Problem solved",
        "✨ مساعدة الذكاء الاصطناعي للصياغة": "✨ AI wording assistance",
        "⟵ ترجمة إلى العربية": "⟵ Translate to Arabic",
        "⭐ تقييم الخدمة": "⭐ Service rating",
        "⭐ قيّم الفني": "⭐ Rate technician",
        "🌐 English": "🌐 English",
        "🏢 إدارة الجهات وشعاراتها": "🏢 Client management",
        "📋 إدارة التذاكر": "📋 Ticket management",
        "📜 عرض الاتفاقية": "📜 View agreement",
        "📥 تصدير Excel": "📥 Export Excel",
        "🖨️ تقرير PDF / طباعة": "🖨️ PDF report / print",
        "🖨️ تقرير التذكرة PDF / طباعة": "🖨️ Ticket report PDF / print",
        "🖨️ طباعة": "🖨️ Print",
        "🖨️ طباعة / حفظ كـ PDF": "🖨️ Print / Save as PDF"
    };

    var AR = /[\u0600-\u06FF]/;
    var ROLES = { 'موظف': 'Employee', 'فني / مهندس': 'Technician / Engineer', 'فني': 'Technician', 'مدير الاستقبال': 'Reception manager', 'مدير النظام': 'System administrator', 'النظام': 'System' };
    var ROLE_RE = '(موظف|فني / مهندس|مدير الاستقبال|مدير النظام|النظام)';
    var MONTHS = { 'يناير': 'January', 'فبراير': 'February', 'مارس': 'March', 'أبريل': 'April', 'مايو': 'May', 'يونيو': 'June', 'يوليو': 'July', 'أغسطس': 'August', 'سبتمبر': 'September', 'أكتوبر': 'October', 'نوفمبر': 'November', 'ديسمبر': 'December' };

    function plural(n, one, many) { return Number(n) === 1 ? one : many; }

    // ترجمة الأزمنة: 2 ساعة 15 دقيقة، ساعتان، يوم واحد، 30 دقيقة ...
    function dur(s) {
        return s
            .replace(/يوم واحد/g, '1 day')
            .replace(/يومان/g, '2 days')
            .replace(/ساعة واحدة/g, '1 hour')
            .replace(/ساعتان/g, '2 hours')
            .replace(/(\d+)\s*يوماً/g, function (m, n) { return n + ' ' + plural(n, 'day', 'days'); })
            .replace(/(\d+)\s*أيام/g, '$1 days')
            .replace(/(\d+)\s*يوم/g, function (m, n) { return n + ' ' + plural(n, 'day', 'days'); })
            .replace(/(\d+)\s*ساعات/g, '$1 hours')
            .replace(/(\d+)\s*ساعة/g, function (m, n) { return n + ' ' + plural(n, 'hour', 'hours'); })
            .replace(/(\d+)\s*دقيقة/g, function (m, n) { return n + ' ' + plural(n, 'minute', 'minutes'); })
            .replace(/\s+و\s*(?=\d)/g, ' and ')
            .replace(/\sو(?=\d)/g, ' and ');
    }

    function role(r) { return ROLES[r] || r; }

    var PATTERNS = [
        [/^تم إيقاف الدخول مؤقتاً بسبب محاولات خاطئة متكررة\. حاول بعد (\d+) دقيقة\.$/, function (m) { return 'Sign-in is temporarily disabled due to repeated failed attempts. Try again in ' + m[1] + ' minute(s).'; }],
        [/^تمت إضافة العميل «(.*)»\.$/, function (m) { return 'Client “' + m[1] + '” added.'; }],
        [/^تم تفعيل العميل «(.*)»\.$/, function (m) { return 'Client “' + m[1] + '” activated.'; }],
        [/^تم إيقاف العميل «(.*)» ولن يستطيع موظفوه الدخول\.$/, function (m) { return 'Client “' + m[1] + '” suspended; its employees can no longer sign in.'; }],
        [/^تمت إضافة المشروع «(.*)»\.$/, function (m) { return 'Project “' + m[1] + '” added.'; }],
        [/^تم تفعيل المشروع «(.*)»\.$/, function (m) { return 'Project “' + m[1] + '” activated.'; }],
        [/^تم إيقاف المشروع «(.*)»\.$/, function (m) { return 'Project “' + m[1] + '” suspended.'; }],
        [/^تعذّرت الترجمة \(رمز الخطأ (\d+)\)\. تحقق من مفتاح الخدمة\.$/, function (m) { return 'Translation failed (error code ' + m[1] + '). Check the service key.'; }],
        [/^قيّمك (الموظف|مدير الاستقبال) ضمن تقييم (.+): ([\d.]+) من 5$/, function (m) { return (m[1] === 'الموظف' ? 'An employee' : 'A reception manager') + ' rated you in the ' + tr(m[2]) + ' evaluation: ' + m[3] + ' out of 5'; }],
        [/^نوع الملف غير مسموح: (.*)\. المسموح: صور وPDF وWord وExcel ونص\.$/, function (m) { return 'File type not allowed: ' + m[1] + '. Allowed: images, PDF, Word, Excel and text.'; }],
        [/^حجم الملف يتجاوز 10 ميجابايت: (.*)$/, function (m) { return 'File size exceeds 10 MB: ' + m[1]; }],
        [/^محتوى الملف لا يطابق امتداده: (.*)$/, function (m) { return 'The file content does not match its extension: ' + m[1]; }],
        [/^الحد الأقصى (\d+) ملفات في المرة الواحدة\.$/, function (m) { return 'Maximum ' + m[1] + ' files at a time.'; }],
        [/^تذكرة جديدة #(\d+): (.*) — بانتظار قرارك$/, function (m) { return 'New ticket #' + m[1] + ': ' + m[2] + ' — awaiting your decision'; }],
        [/^تعليق جديد من الموظف على التذكرة #(\d+)$/, function (m) { return 'New comment from the employee on ticket #' + m[1]; }],
        [/^تعليق جديد من الفني على التذكرة #(\d+)$/, function (m) { return 'New comment from the technician on ticket #' + m[1]; }],
        [/^تعليق جديد على التذكرة #(\d+) من (.+)$/, function (m) { return 'New comment on ticket #' + m[1] + ' from ' + m[2]; }],
        [/^رد جديد على تذكرتك #(\d+)$/, function (m) { return 'New reply on your ticket #' + m[1]; }],
        [/^رسالة جديدة بخصوص التذكرة #(\d+)$/, function (m) { return 'New message about ticket #' + m[1]; }],
        [/^قبول التذكرة وتحديد مستوى التأثير \((.+)\) وتوجيهها إلى (.+) — مهلة الحل (.+) وبدأ عدّاد SLA \((.+)\)$/, function (m) { return 'Ticket accepted, impact level set (' + tr(m[1]) + ') and assigned to ' + m[2] + ' — resolution deadline ' + dur(m[3]) + '; SLA counter started (' + tr(m[4]) + ')'; }],
        [/^موافقة وتوجيه التذكرة إلى (.+) وبدء عداد SLA$/, function (m) { return 'Ticket approved and assigned to ' + m[1] + '; SLA counter started'; }],
        [/^تم توجيه التذكرة #(\d+) إليك — مستوى التأثير: (.+)، مهلة الحل: (.+)$/, function (m) { return 'Ticket #' + m[1] + ' was assigned to you — impact level: ' + tr(m[2]) + ', resolution deadline: ' + dur(m[3]); }],
        [/^تم توجيه التذكرة #(\d+) إليك — بدأ عداد SLA$/, function (m) { return 'Ticket #' + m[1] + ' was assigned to you — the SLA counter has started'; }],
        [/^تم قبول تذكرتك #(\d+) وتوجيهها للمختص$/, function (m) { return 'Your ticket #' + m[1] + ' was accepted and assigned to a specialist'; }],
        [/^تمت الموافقة على تذكرتك #(\d+) وتوجيهها إلى (.+)$/, function (m) { return 'Your ticket #' + m[1] + ' was approved and assigned to ' + m[2]; }],
        [/^تم رفض تذكرتك #(\d+)\. السبب: (.*)$/, function (m) { return 'Your ticket #' + m[1] + ' was rejected. Reason: ' + m[2]; }],
        [/^تم رفض تذكرتك #(\d+)$/, function (m) { return 'Your ticket #' + m[1] + ' was rejected'; }],
        [/^الفني أرسل حل التذكرة #(\d+) بانتظار اعتمادك$/, function (m) { return 'The technician sent the solution of ticket #' + m[1] + ' for your approval'; }],
        [/^تم حل المشكلة في تذكرتك #(\d+) وإغلاقها — يمكنك الآن تقييم الفني إن كان التقييم متاحاً$/, function (m) { return 'The problem in your ticket #' + m[1] + ' is solved and the ticket is closed'; }],
        [/^تم حل تذكرتك #(\d+) وإغلاقها — يمكنك الآن تقييم الفني$/, function (m) { return 'Your ticket #' + m[1] + ' was solved and closed'; }],
        [/^اعتمد مدير الاستقبال حلّك للتذكرة #(\d+)$/, function (m) { return 'The reception manager approved your solution for ticket #' + m[1]; }],
        [/^أُعيد الحل للمعالجة: (.*)$/, function (m) { return 'The solution was returned for further work: ' + m[1]; }],
        [/^أعاد مدير الاستقبال حل التذكرة #(\d+) للمعالجة: (.*)$/, function (m) { return 'The reception manager returned the solution of ticket #' + m[1] + ' for further work: ' + m[2]; }],
        [/^قيّم الموظف أداءك في التذكرة #(\d+): (\d+) من 5$/, function (m) { return 'The employee rated your performance on ticket #' + m[1] + ': ' + m[2] + ' out of 5'; }],
        [/^ردّ مدير النظام على تقييمك للتذكرة #(\d+)$/, function (m) { return 'The system administrator replied to your rating of ticket #' + m[1]; }],
        [/^تمت إضافة المستخدم (.+)\.$/, function (m) { return 'User ' + m[1] + ' added.'; }],
        [/^تم تحديث جهة (.+)\.$/, function (m) { return 'Client of ' + m[1] + ' updated.'; }],
        [/^تم تفعيل (.+)\.$/, function (m) { return m[1] + ' activated.'; }],
        [/^تم إيقاف (.+)\.$/, function (m) { return m[1] + ' suspended.'; }],
        [/^تم تغيير كلمة مرور (.+)\.$/, function (m) { return 'Password of ' + m[1] + ' changed.'; }],
        [/^⚠️ تجاوزت التذكرة #(\d+) مهلة SLA(?: — ما زالت قيد المعالجة| ولم تُغلق بعد)$/, function (m) { return '⚠️ Ticket #' + m[1] + ' exceeded the SLA deadline — still in progress'; }],
        [/^⏳ اقتربت مهلة SLA للتذكرة #(\d+) — متبقي (.+)$/, function (m) { return '⏳ The SLA deadline of ticket #' + m[1] + ' is approaching — remaining ' + dur(m[2]); }],
        [/^النصف الأول (\d{4}) \(يناير – يونيو\)$/, function (m) { return 'First half ' + m[1] + ' (January – June)'; }],
        [/^النصف الثاني (\d{4}) \(يوليو – ديسمبر\)$/, function (m) { return 'Second half ' + m[1] + ' (July – December)'; }],
        [/^عام (\d{4})$/, function (m) { return 'Year ' + m[1]; }],
        [/^(يناير|فبراير|مارس|أبريل|مايو|يونيو|يوليو|أغسطس|سبتمبر|أكتوبر|نوفمبر|ديسمبر) (\d{4})$/, function (m) { return MONTHS[m[1]] + ' ' + m[2]; }],
        [/^([\d.]+) س$/, function (m) { return m[1] + ' h'; }],
        [/^تحديد الكل كمقروء \((\d+)\)$/, function (m) { return 'Mark all as read (' + m[1] + ')'; }],
        [/^تذاكر أُغلقت في هذه الدورة: (\d+)$/, function (m) { return 'Tickets closed in this cycle: ' + m[1]; }],
        [/^(.+) — الحالية$/, function (m) { return tr(m[1]) + ' — current'; }],
        [/^تقييم (الموظف|مدير الاستقبال)$/, function (m) { return 'Rating by ' + (m[1] === 'الموظف' ? 'employee' : 'reception manager'); }],
        [/^— للتواصل: (.*)$/, function (m) { return '— Contact: ' + m[1]; }],
        [/^مرحباً: (.+) \((.+)\)$/, function (m) { return 'Welcome: ' + m[1] + ' (' + role(m[2]) + ')'; }],
        [/^تفاصيل التذكرة رقم: (\d+)$/, function (m) { return 'Ticket details no.: ' + m[1]; }],
        [/^تقرير التذكرة رقم (\d+)$/, function (m) { return 'Ticket report no. ' + m[1]; }],
        [/^\(المطلوب خلال (.+)\)$/, function (m) { return '(required within ' + dur(m[1]) + ')'; }],
        [/^السبب: (.*)$/, function (m) { return 'Reason: ' + m[1]; }],
        [/^تقرير الفني \((.+)\)$/, function (m) { return 'Technician report (' + tr('(' + m[1] + ')').replace(/^\(|\)$/g, '') + ')'; }],
        [/^(حرج|عالي|متوسط|منخفض) \((Critical|High|Medium|Low)\) — الاستجابة خلال (.+) والحل خلال (.+)$/, function (m) { return m[2] + ' — response within ' + dur(m[3]) + ' and resolution within ' + dur(m[4]); }],
        [/^— رفعه: (.+)، (\d{4}-\d{2}-\d{2} \d{2}:\d{2})$/, function (m) { return '— Uploaded by: ' + role(m[1]) + ', ' + m[2]; }],
        [new RegExp('^(.+) — ' + ROLE_RE + '، (\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2})$'), function (m) { return m[1] + ' — ' + role(m[2]) + ', ' + m[3]; }],
        [new RegExp('^— ' + ROLE_RE + ' ?(.*)، (\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2})$'), function (m) { return '— ' + role(m[1]) + ' ' + m[2] + ', ' + m[3]; }],
        [new RegExp('^' + ROLE_RE + ' — (.+)$'), function (m) { return role(m[1]) + ' — ' + m[2]; }],
        [/^حُلّت ضمن الوقت \((.+)\)$/, function (m) { return 'Solved within the deadline (' + dur(m[1]) + ')'; }],
        [/^حُلّت متأخرة بـ (.+)$/, function (m) { return 'Solved late by ' + dur(m[1]); }],
        [/^أُوقف العداد ضمن الوقت \((.+)\)$/, function (m) { return 'Counter stopped within the deadline (' + dur(m[1]) + ')'; }],
        [/^أُغلقت متأخرة بـ (.+)$/, function (m) { return 'Closed late by ' + dur(m[1]); }],
        [/^تجاوزت المهلة بـ (.+)$/, function (m) { return 'Exceeded the deadline by ' + dur(m[1]); }],
        [/^متأخرة (.+)$/, function (m) { return 'Late ' + dur(m[1]); }],
        [/^متبقي (.+)$/, function (m) { return 'Remaining ' + dur(m[1]); }],
        [/^يعادل: (.+)$/, function (m) { return 'Equals: ' + dur(m[1]); }],
        [/^عدد النتائج: (\d+)$/, function (m) { return 'Results: ' + m[1]; }],
        [/^نتيجة إضافة موظفي «(.+)»$/, function (m) { return 'Result of adding employees of “' + m[1] + '”'; }],
        [/^بحث: (.*)$/, function (m) { return 'Search: ' + m[1]; }],
        [/^الحالة: (.+)$/, function (m) { return 'Status: ' + tr(m[1]); }],
        [/^الخطورة: (.+)$/, function (m) { return 'Severity: ' + tr(m[1]); }],
        [/^موظف (مدن|وزارة الصحة|منشآت|المحمية) (\d+)$/, function (m) { return 'Employee ' + ({ 'مدن': 'MODON', 'وزارة الصحة': 'Ministry of Health', 'منشآت': "Monsha'at", 'المحمية': 'Reserve' }[m[1]]) + ' ' + m[2]; }],
        [new RegExp('^(.+) [–-] ' + ROLE_RE + '$'), function (m) { return m[1] + ' – ' + role(m[2]); }],
        [/^(\d+) (شهر|يوم)$/, function (m) { var n = m[1]; return n + ' ' + (m[2] === 'شهر' ? plural(n, 'month', 'months') : plural(n, 'day', 'days')); }]
    ];

    var PUNCT = { ':': ':', '：': ':', '.': '.', '،': ',', '؛': ';', '!': '!', '؟': '?', '?': '?' };

    function tr(core) {
        if (Object.prototype.hasOwnProperty.call(D, core)) { return D[core]; }

        var i, m;
        for (i = 0; i < PATTERNS.length; i++) {
            m = PATTERNS[i][0].exec(core);
            if (m) { return PATTERNS[i][1](m); }
        }

        // علامة ترقيم في النهاية ( : . ، ؟ )
        m = /^(.+?)([:：.،؛!؟?]+)$/.exec(core);
        if (m && AR.test(m[1])) {
            var base = tr(m[1]);
            if (base !== m[1]) {
                var p = '';
                for (i = 0; i < m[2].length; i++) { p += (PUNCT[m[2][i]] || m[2][i]); }
                return base + p;
            }
        }

        // رموز في البداية (● — ✅ ...)
        m = /^([^\u0600-\u06FFA-Za-z0-9«(#]+)(.+)$/.exec(core);
        if (m && AR.test(m[2])) {
            var rest = tr(m[2]);
            if (rest !== m[2]) { return m[1] + rest; }
        }

        // أزمنة بأرقام
        if (/\d/.test(core) && /(دقيقة|ساعة|ساعات|يوم|أيام|يوماً|ساعتان|يومان)/.test(core)) {
            var d = dur(core);
            if (!AR.test(d)) { return d; }
        }

        return core;
    }

    function translateText(text) {
        if (!AR.test(text)) { return text; }
        var m = /^(\s*)([\s\S]*?)(\s*)$/.exec(text);
        var core = m[2].replace(/\s+/g, ' ');
        var out = tr(core);
        return out === core ? text : m[1] + out + m[3];
    }

    window.__tr = function (s) { return translateText(String(s)); };

    var SKIP = { SCRIPT: 1, STYLE: 1, TEXTAREA: 1, CODE: 1, NOSCRIPT: 1 };
    var ATTRS = ['placeholder', 'title', 'aria-label', 'alt'];

    function attrs(el) {
        var i, v, t;
        for (i = 0; i < ATTRS.length; i++) {
            v = el.getAttribute(ATTRS[i]);
            if (v && AR.test(v)) {
                t = translateText(v);
                if (t !== v) { el.setAttribute(ATTRS[i], t); }
            }
        }
        if (el.tagName === 'INPUT' && /^(submit|button|reset)$/i.test(el.type)) {
            v = el.getAttribute('value');
            if (v && AR.test(v)) {
                t = translateText(v);
                if (t !== v) { el.setAttribute('value', t); }
            }
        }
        if (el.getAttribute('dir') === 'rtl' && !el.hasAttribute('data-keep-dir')) {
            el.setAttribute('dir', 'ltr');
        }
    }

    function walk(node) {
        if (node.nodeType === 3) {
            var nv = node.nodeValue;
            if (AR.test(nv)) {
                var t = translateText(nv);
                if (t !== nv) { node.nodeValue = t; }
            }
            return;
        }
        if (node.nodeType !== 1) { return; }
        if (SKIP[node.tagName] || node.hasAttribute('data-notranslate')) { return; }
        attrs(node);
        for (var c = node.firstChild; c; c = c.nextSibling) { walk(c); }
    }

    // ما يعرضه alert و confirm
    var _alert = window.alert, _confirm = window.confirm;
    window.alert = function (m) { return _alert.call(window, translateText(String(m))); };
    window.confirm = function (m) { return _confirm.call(window, translateText(String(m))); };

    function translateTitle() {
        var parts = document.title.split(' - ');
        var out = parts.map(function (p) { return translateText(p); }).join(' - ');
        if (out !== document.title) { document.title = out; }
    }

    function start() {
        walk(document.body);
        translateTitle();
        setButton();

        new MutationObserver(function (muts) {
            for (var i = 0; i < muts.length; i++) {
                var m = muts[i];
                if (m.type === 'characterData') {
                    var p = m.target.parentNode;
                    if (p && !SKIP[p.tagName] && AR.test(m.target.nodeValue)) {
                        var t = translateText(m.target.nodeValue);
                        if (t !== m.target.nodeValue) { m.target.nodeValue = t; }
                    }
                } else {
                    for (var j = 0; j < m.addedNodes.length; j++) { walk(m.addedNodes[j]); }
                }
            }
        }).observe(document.body, { childList: true, subtree: true, characterData: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', start);
    } else {
        start();
    }
})();