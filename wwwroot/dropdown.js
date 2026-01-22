// Initialize Bootstrap-like dropdown functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeDropdowns();
});

function initializeDropdowns() {
    // Handle dropdown toggle clicks
    document.addEventListener('click', function(event) {
        const dropdownToggle = event.target.closest('[data-bs-toggle="dropdown"]');
        
        if (dropdownToggle) {
            event.preventDefault();
            const dropdownMenu = dropdownToggle.nextElementSibling;
            
            if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                // Close all other dropdowns
                document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                    if (menu !== dropdownMenu) {
                        menu.classList.remove('show');
                    }
                });
                
                // Toggle current dropdown
                dropdownMenu.classList.toggle('show');
            }
        } else {
            // Click outside - close all dropdowns
            if (!event.target.closest('.dropdown-menu')) {
                document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                    menu.classList.remove('show');
                });
            }
        }
    });
}

// Re-initialize when Blazor updates the DOM
if (typeof MutationObserver !== 'undefined') {
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.addedNodes.length) {
                initializeDropdowns();
            }
        });
    });
    
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
}
