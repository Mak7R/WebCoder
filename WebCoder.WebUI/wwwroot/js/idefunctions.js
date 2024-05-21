function fetchStructure(userName, title) {
    return fetch(`/repositories/${userName}/${title}/sources/structure`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            renderStructure(data.result);
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
}

function renderStructure(structure) {
    const aside = document.getElementById('file-explorer-tree');
    aside.innerHTML = '';
    renderNode(structure, aside, '');
    afterRender();
}

function renderNode(node, parentElement, path) {
    const item = document.createElement('li');
    const itemName = document.createElement('span');
    const itemIcon = document.createElement('img')
    const  itemContent = document.createElement('span')

    itemIcon.classList.add('file-explorer-item-icon')
    if (path === '' && node.type === 'Repository') {
        itemIcon.src = '/icons/icon-repository.png';
    } else if (node.type === 'Directory') {
        itemIcon.src = '/icons/icon-directory.svg';
    } else if (node.type === 'File') {
        itemIcon.src = '/icons/icon-file.svg';
    }
    
    itemName.textContent = node.name;

    let cur_path = path + '/' + node.name;
    
    itemContent.classList.add('file-explorer-item-content');
    
    itemContent.appendChild(itemIcon);
    itemContent.appendChild(itemName);

    itemContent.setAttribute('data-bs-toggle', 'popover');
    itemContent.setAttribute('data-bs-trigger', 'hover focus');
    itemContent.setAttribute('data-bs-content', cur_path);

    itemContent.addEventListener("click", function(event) {
        if (event.ctrlKey){
            let path = this.getAttribute('data-bs-content');

            navigator.clipboard.writeText(path).then(function() {
                console.log("Значение успешно скопировано в буфер обмена: " + valueToCopy);
            }, function(err) {
                console.error("Ошибка при копировании значения: ", err);
            });
        }
    });
    
    new bootstrap.Popover(itemContent);
    
    item.classList.add('file-explorer-item');
    item.appendChild(itemContent);
    item.setAttribute('path', cur_path);

    

    // Рекурсивно обрабатываем дочерние элементы, если они есть
    if (node.children && node.children.length > 0) {
        itemContent.classList.add('caret')
        const childList = document.createElement('ul');
        childList.classList.add('nested')
        node.children.forEach(child => {
            renderNode(child, childList, cur_path);
        });
        item.appendChild(childList);
    }

    parentElement.appendChild(item);
}

function afterRender(){
    // init tree settings
    let togglers = document.getElementsByClassName("caret");
    for (let i = 0; i < togglers.length; i++) {
        togglers[i].addEventListener("click", function(event) {
            if (!event.ctrlKey){
                this.parentElement.querySelector(".nested").classList.toggle("active");
                this.classList.toggle("caret-down");
            }
        });
    }
}