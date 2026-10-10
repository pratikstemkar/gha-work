import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import ItemsView from '@/views/ItemsView.vue'
import type { Item } from '@/types'

const itemsApi = vi.hoisted(() => ({
  fetchItems: vi.fn(),
  createItem: vi.fn(),
  setItemDone: vi.fn(),
  deleteItem: vi.fn(),
}))

vi.mock('@/api/items', () => itemsApi)

const alpha: Item = { id: 1, title: 'Alpha', done: false, createdAt: '2026-01-01T00:00:00Z' }

describe('ItemsView', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders the items returned by the API', async () => {
    itemsApi.fetchItems.mockResolvedValue([alpha])

    const wrapper = mount(ItemsView)
    await flushPromises()

    expect(wrapper.text()).toContain('Alpha')
    expect(wrapper.findAll('li')).toHaveLength(1)
  })

  it('adds an item and prepends it to the list', async () => {
    itemsApi.fetchItems.mockResolvedValue([alpha])
    itemsApi.createItem.mockResolvedValue({
      id: 2,
      title: 'Beta',
      done: false,
      createdAt: '2026-01-02T00:00:00Z',
    })

    const wrapper = mount(ItemsView)
    await flushPromises()

    await wrapper.find('form input').setValue('Beta')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(itemsApi.createItem).toHaveBeenCalledWith('Beta')
    expect(wrapper.text()).toContain('Beta')
    expect((wrapper.find('form input').element as HTMLInputElement).value).toBe('')
  })

  it('does not add a blank item', async () => {
    itemsApi.fetchItems.mockResolvedValue([])

    const wrapper = mount(ItemsView)
    await flushPromises()

    await wrapper.find('form input').setValue('   ')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(itemsApi.createItem).not.toHaveBeenCalled()
  })

  it('marks an item as done', async () => {
    itemsApi.fetchItems.mockResolvedValue([alpha])
    itemsApi.setItemDone.mockResolvedValue({ ...alpha, done: true })

    const wrapper = mount(ItemsView)
    await flushPromises()

    await wrapper.find('input[type="checkbox"]').trigger('change')
    await flushPromises()

    expect(itemsApi.setItemDone).toHaveBeenCalledWith(1, true)
    expect(wrapper.find('span.flex-1').classes()).toContain('line-through')
  })

  it('removes an item', async () => {
    itemsApi.fetchItems.mockResolvedValue([alpha])
    itemsApi.deleteItem.mockResolvedValue(undefined)

    const wrapper = mount(ItemsView)
    await flushPromises()

    await wrapper.find('li button').trigger('click')
    await flushPromises()

    expect(itemsApi.deleteItem).toHaveBeenCalledWith(1)
    expect(wrapper.findAll('li')).toHaveLength(0)
  })
})
