import { CellComponent, ColumnDefinition, RowComponent, TabulatorFull as Tabulator } from 'tabulator-tables';
import 'tabulator-tables/dist/css/tabulator_bootstrap5.min.css';
import '../styles/data-table.scss';

interface FormDataColumn {
  title: string;
  field: string;
  sorter: 'number' | 'string';
}

interface FormDataRow extends Record<string, unknown> {
  _itemId: number;
}

interface FormDataConfig {
  columns: FormDataColumn[];
  rows: FormDataRow[];
}

interface FormDataOptions {
  tableId: string;
  configId: string;
  rowArtifactsId: string;
  searchId: string;
  pageSizeId: string;
  columnChooserId: string;
  persistenceId: string;
}

function initDataTable(options: FormDataOptions) {
  const tableElement = document.getElementById(options.tableId);
  const configElement = document.getElementById(options.configId);
  const artifactStore = document.getElementById(options.rowArtifactsId);
  const searchElement = document.getElementById(options.searchId) as HTMLInputElement | null;
  const pageSizeElement = document.getElementById(options.pageSizeId) as HTMLSelectElement | null;
  const columnChooser = document.getElementById(options.columnChooserId);

  if (
    !tableElement
    || !configElement
    || !artifactStore
    || !searchElement
    || !pageSizeElement
    || !columnChooser
    || tableElement.dataset.mobiusTableInitialized
  ) return;

  tableElement.dataset.mobiusTableInitialized = 'true';
  const config = JSON.parse(configElement.textContent || '{}') as FormDataConfig;

  // Short table values
  const textFormatter = (cell: CellComponent): HTMLElement => {
    const value = cell.getValue();
    const text = value == null ? '' : String(value);
    const span = document.createElement('span');
    span.textContent = text.length > 40 ? `${text.slice(0, 37)}…` : text;
    if (text.length > 40) span.title = text;
    return span;
  };

  // Add files from Razor
  const filesFormatter = (cell: CellComponent): HTMLElement => {
    const itemId = (cell.getRow().getData() as FormDataRow)._itemId;
    const artifact = artifactStore.querySelector<HTMLElement>(`[data-mobius-files-artifact="${itemId}"]`);
    if (!artifact) return document.createElement('span');
    return artifact.cloneNode(true) as HTMLElement;
  };

  // Dynamic columns
  const columns: ColumnDefinition[] = config.columns.map(column => ({
    ...column,
    headerFilter: 'input',
    headerFilterPlaceholder: 'Filter…',
    minWidth: column.field === 'Id' || column.field === 'Files' ? 90 : 140,
    hozAlign: column.field === 'Id' || column.field === 'Files' ? 'right' : 'left',
    formatter: column.field === 'Files' ? filesFormatter : textFormatter,
  }));

  const table = new Tabulator(tableElement, {
    data: config.rows,
    columns,
    index: '_itemId',
    height: '70vh',
    layout: 'fitData',
    renderHorizontal: 'virtual',
    nestedFieldSeparator: false,
    pagination: true,
    paginationMode: 'local',
    paginationSize: Number(pageSizeElement.value),
    paginationCounter: 'rows',
    placeholder: 'No submissions found',
    persistence: {
      columns: ['visible', 'width'],
      sort: true,
      headerFilter: true,
    },
    persistenceID: options.persistenceId,
    rowFormatter: (row: RowComponent) => {
      const itemId = (row.getData() as FormDataRow)._itemId;
      const artifact = artifactStore.querySelector<HTMLElement>(`[data-mobius-row-artifact="${itemId}"]`);
      if (!artifact) 
        return;

      const rowElement = row.getElement();
      for (const attribute of artifact.attributes) {
        if (!['id', 'class', 'style', 'hidden', 'data-mobius-row-artifact'].includes(attribute.name)) {
          rowElement.setAttribute(attribute.name, attribute.value);
        }
      }
    },
  });

  const applyGlobalSearch = (): void => {
    const search = searchElement.value.trim().toLocaleLowerCase();
    table.clearFilter(false);
    if (!search) 
      return;

    table.setFilter((row: FormDataRow) => config.columns.some(column => {
      const value = row[column.field];
      return value != null && String(value).toLocaleLowerCase().includes(search);
    }));
  };

  searchElement.addEventListener('input', applyGlobalSearch);
  pageSizeElement.addEventListener('change', () => table.setPageSize(Number(pageSizeElement.value)));

  const buildColumnChooser = (): void => {
    columnChooser.replaceChildren();

    for (const columnConfig of config.columns) {
      const column = table.getColumn(columnConfig.field);
      if (!column) 
        continue;

      const label = document.createElement('label');
      label.className = 'd-flex align-items-center gap-2 py-1';

      const checkbox = document.createElement('input');
      checkbox.type = 'checkbox';
      checkbox.className = 'form-check-input mt-0';
      checkbox.checked = column.isVisible();
      checkbox.addEventListener('change', () => column.toggle());

      const text = document.createElement('span');
      text.textContent = columnConfig.title;

      label.append(checkbox, text);
      columnChooser.append(label);
    }
  };

  table.on('tableBuilt', buildColumnChooser);
}

var winAny = window as any;
winAny.appMobius6 ??= {};
winAny.appMobius6.initDataTable ??= initDataTable;

