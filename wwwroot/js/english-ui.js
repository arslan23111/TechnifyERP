(() => {
  const text = {
    "Registered students aur approved courses dekhein.": "View registered students and their approved courses.",
    "Students aur unke course enrollment statuses.": "Review students and their course enrollment statuses.",
    "Institute ke courses, credit hours aur monthly fees manage karein.": "Manage institute courses, credit hours, and monthly fees.",
    "Faculty profiles, departments aur account status dekhein.": "View faculty profiles, departments, and account status.",
    "Pending Student aur Faculty registration requests review karein.": "Review pending student and faculty registration requests.",
    "Course-wise student attendance summary. Leave percentage mein count nahi hoti.": "Course-wise student attendance summary. Leave records are not included in the percentage.",
    "Monthly dues aur submitted receipts manage karein.": "Manage monthly dues and submitted payment receipts.",
    "Student requests review karein.": "Review student installment requests.",
    "All students, course ya specific student ko notice bhejein.": "Send notices to all students, a course, or a specific student.",
    "Requirement ke mutabiq course ki basic academic aur fee information enter karein.": "Enter the course's basic academic and fee information.",
    "Requirement ke mutabiq available course self-assign karein.": "Self-assign an available course.",
    "Assigned course aur date select karke student attendance mark karein.": "Select an assigned course and date to record student attendance.",
    "Assigned courses ke student questions answer karein.": "Answer student questions from your assigned courses.",
    "Timed MCQ tests manage karein.": "Create and manage timed multiple-choice tests.",
    "Study materials aur recorded lectures manage karein.": "Manage study materials and recorded lectures.",
    "Course programming problems create karein.": "Create programming challenges for your courses.",
    "Assigned course ke students ko notice bhejein.": "Send notices to students enrolled in your assigned courses.",
    "Assigned classes, timings aur rooms.": "View assigned classes, times, and rooms.",
    "Schedule available nahi.": "No schedule is available.",
    "Apne courses ke assignments manage karein.": "Manage assignments for your assigned courses.",
    "Abhi koi assignment nahi.": "No assignments have been created.",
    "Apne course ke eligible student ko achievement badge dein.": "Award an achievement badge to an eligible student in your course.",
    "Apne enrolled courses ke materials download aur lectures watch karein.": "Download materials and watch lectures for your enrolled courses.",
    "Language choose karke apna code submit karein.": "Choose a language and submit your code.",
    "Private notes sirf aapko nazar aayengi.": "Private notes are visible only to you.",
    "Enrolled courses ka weekly timetable.": "Your weekly timetable for enrolled courses.",
    "Aap ke liye koi notice nahi.": "There are no notices for you.",
    "Latest institute aur course announcements.": "Latest institute and course announcements.",
    "Submit karein aur grades check karein.": "Submit your work and check your grades.",
    "Abhi koi assignment available nahi.": "No assignments are currently available.",
    "Receipt upload karein ya installment request bhejein.": "Upload a payment receipt or submit an installment request.",
    "Receipt admin review mein hai.": "Your receipt is under administrator review.",
    "Request status yahan check karein.": "Check the status of your requests here.",
    "Abhi koi request nahi.": "There are no requests yet.",
    "Abhi koi certificate issue nahi hua.": "No certificates have been issued yet.",
    "Aap ne abhi koi badge earn nahi kiya.": "You have not earned any badges yet.",
    "Request approve karne par old enrollment close aur new course ki fee generate hoti hai.": "Approving a request closes the previous enrollment and creates a fee record for the new course.",
    "Naye course ke liye enrollment request bhejein aur status check karein.": "Request enrollment in a new course and check its status.",
    "Approved course ko doosre available course mein change karne ki request bhejein.": "Request a change from an approved course to another available course.",
    "Public homepage ka hero aur footer content yahan se update karein.": "Update public homepage hero and footer content here.",
    "Homepage content save ho gaya.": "Homepage content has been saved.",
    "Homepage stats aur feature cards save ho gaye.": "Homepage statistics and feature cards have been saved.",
    "Inbox empty hai.": "Your inbox is empty.",
    "Apne available contacts ko direct message bhejein.": "Send a direct message to an available contact."
  };
  function translate(root = document.body) {
    if (!root) return;
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
    const nodes = [];
    while (walker.nextNode()) nodes.push(walker.currentNode);
    nodes.forEach(node => {
      const value = node.nodeValue.trim();
      if (text[value]) node.nodeValue = node.nodeValue.replace(value, text[value]);
    });
  }
  document.addEventListener("DOMContentLoaded", () => {
    translate();
    new MutationObserver(() => translate()).observe(document.body, { childList: true, subtree: true });
  });
})();
